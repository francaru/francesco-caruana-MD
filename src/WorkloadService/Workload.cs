using CommandLine;
using Messaging;
using Messaging.LifecycleEvents;
using Messaging.WorkEvents;

namespace WorkloadService;

public class Workload
{
    const string serviceName = "WorkloadService";

    public static async Task TradeDoWork(IMessageHandler mqClient, MQEventInfo eventInfo, TradeDoWorkEventBody? eventBody, Func<Task>? delay = null)
    {
        Console.WriteLine($"Starting trade: {eventBody!.TradeId}");

        mqClient.Produce(
            toQueues: ["OperationalService.BusinessLogic.Service.TradesService::OnStatusChange"],
            new TradeStatusChangeEventBody() {
                TradeId = eventBody!.TradeId, 
                PreviousStatus = "starting", 
                NewStatus = "running" 
            }
        );

        delay ??= () => Task.Delay(10_000);
        await delay();

        mqClient.Produce(
            toQueues: ["OperationalService.BusinessLogic.Service.TradesService::OnStatusChange"],
            new TradeStatusChangeEventBody()
            {
                TradeId = eventBody!.TradeId,
                PreviousStatus = "running",
                NewStatus = "complete"
            }
        );
        
        Console.WriteLine($"Completed trade: {eventBody!.TradeId}");
    }

    class Options
    {
        [Option("rabbitmq-host", HelpText = "Host where to connect to RabbitMQ", Default = "localhost")]
        public required string RabbitMQHost { get; set; }
    }

    static async Task Main(string[] args)
    {
        var options = Parser.Default.ParseArguments<Options>(args).Value;

        using var mqClient = MQClient
            .Connect(hostName: options.RabbitMQHost, serviceName: serviceName)
            .Subscribe();

        await Task.Delay(Timeout.Infinite);
    }
}
