using Database;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Messaging;

public class MQClient : IDisposable
{
    private static MQClient? instance;
    private readonly ConnectionFactory factory;
    private readonly IConnection connection;
    private readonly IChannel channel;

    private IServiceProvider? ServiceProvider { get; init; }

    public string ServiceName { get; init; }

    MQClient(string serviceName, IServiceProvider? serviceProvider) 
    {
        factory = new() { HostName = "localhost" };
        connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
        channel = connection.CreateChannelAsync().GetAwaiter().GetResult();
        
        ServiceName = serviceName;
        ServiceProvider = serviceProvider;
    }

    public virtual void Dispose()
    {
        channel.Dispose();
        connection.Dispose();
    }

    public static MQClient GetInstance()
    {
        if (instance == null)
        {
            throw new InvalidOperationException("First call to GetInstance requires a service name.");
        }

        return Connect(instance.ServiceName, instance.ServiceProvider);
    }

    public static MQClient Connect(string serviceName, IServiceProvider? serviceProvider = null)
    {
        instance ??= new(serviceName: serviceName, serviceProvider: serviceProvider);

        return instance;
    }

    public async void CreateQueue(string queueName)
    {
        await channel.QueueDeclareAsync(
            queue: queueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );
    }

    public async void Consume<T>(string onQueue, params Action<DatabaseContext, ActivitySource, ILoggerProvider, MQEventInfo, T?>[] consumerActions) where T : MQEventBody
    {
        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += (_, eventArgs) =>
        {
            var body = eventArgs.Body.ToArray();
            var message = JsonNode.Parse(Encoding.UTF8.GetString(body));

            if (message is not null)
            {
                var mqEvent = JsonSerializer.Deserialize<MQEvent<T>>(message);

                if (mqEvent is not null)
                {
                    var queueEventInfo = new MQEventInfo() { 
                        QueueName = onQueue, 
                        Sender = mqEvent.Recipient 
                    };

                    foreach (var consumerAction in consumerActions)
                    {
                        new Thread(() =>
                        {
                            using var scope = (ServiceProvider is null) ? null : ServiceProvider.CreateScope();

                            var dbContext = (scope is null) ? null : scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                            var activitySource = (scope is null) ? null : scope.ServiceProvider.GetRequiredService<ActivitySource>();
                            var loggerProvider = (scope is null) ? null : scope.ServiceProvider.GetRequiredService<ILoggerProvider>();

                            consumerAction(dbContext!, activitySource!, loggerProvider!, queueEventInfo, mqEvent.Body);
                        }).Start();
                    }
                }
            }

            return Task.CompletedTask;
        };

        await channel.BasicConsumeAsync(onQueue, autoAck: true, consumer: consumer);
    }

    public async void Produce<T>(string[] toQueues, T mqEventBody) where T : MQEventBody
    {
        var mqRecipient = new MQEventRecipient() {
            ServiceName = ServiceName
        };

        var mqEvent = new MQEvent<T>() { 
            Body = mqEventBody, 
            Recipient = mqRecipient 
        };

        var message = JsonSerializer.Serialize(mqEvent);
        var body = Encoding.UTF8.GetBytes(message);

        foreach (var toQueue in toQueues)
        {
            await channel.BasicPublishAsync(exchange: string.Empty, routingKey: toQueue, body: body);
        }
    }
}
