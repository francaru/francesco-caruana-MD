using RabbitMQ.Client;
using RabbitMQ.Client.Events;
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
    
    private string ServiceName { get; init; }

    MQClient(string serviceName) 
    {
        factory = new() { HostName = "localhost" };
        connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
        channel = connection.CreateChannelAsync().GetAwaiter().GetResult();
        
        ServiceName = serviceName;
    }

    public virtual void Dispose()
    {
        channel.Dispose();
        connection.Dispose();
    }

    public static MQClient GetInstance()
    {
        return GetInstance(Guid.NewGuid().ToString());
    }

    public static MQClient GetInstance(string serviceName)
    {
        instance ??= new(serviceName: serviceName);

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

    public async void Consume<T>(string onQueue, params Action<MQEventInfo, T?>[] consumerActions) where T : MQEventBody
    {
        AsyncEventingBasicConsumer consumer = new(channel);

        consumer.ReceivedAsync += (_, eventArgs) =>
        {
            byte[] body = eventArgs.Body.ToArray();
            JsonNode? message = JsonNode.Parse(Encoding.UTF8.GetString(body));

            if (message is not null)
            {
                MQEvent<T>? mqEvent = JsonSerializer.Deserialize<MQEvent<T>>(message);

                if (mqEvent is not null)
                {
                    MQEventInfo queueEventInfo = new(queueName: onQueue, sender: mqEvent.Recipient);

                    foreach (Action<MQEventInfo, T?> consumerAction in consumerActions)
                    {
                        new Thread(() => consumerAction(queueEventInfo, mqEvent.Body)).Start();
                    }
                }
            }

            return Task.CompletedTask;
        };

        await channel.BasicConsumeAsync(onQueue, autoAck: true, consumer: consumer);
    }

    public async void Produce<T>(string[] toQueues, T mqEventBody) where T : MQEventBody
    {
        MQEventRecipient mqRecipient = new(serviceName: ServiceName);
        MQEvent<T> mqEvent = new(body: mqEventBody, recipient: mqRecipient);

        string message = JsonSerializer.Serialize(mqEvent);
        byte[] body = Encoding.UTF8.GetBytes(message);

        foreach (string toQueue in toQueues)
        {
            await channel.BasicPublishAsync(exchange: string.Empty, routingKey: toQueue, body: body);
        }
    }
}
