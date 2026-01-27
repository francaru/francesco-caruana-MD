using Database;
using Messaging;
using Messaging.LifecycleEvents;
using Messaging.WorkEvents;

namespace WorkloadService;

public class Workload
{
    const string serviceName = "WorkloadService";

    public static void TradeDoWork(MQClient mqClient, MQEventInfo eventInfo, TradeDoWorkEventBody? eventBody)
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

        Thread.Sleep(10000);

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

    static void Main(string[] args)
    {
        using var mqClient = MQClient
            .Connect(serviceName: serviceName)
            .Subscribe();

        Console.ReadKey();
    }
}
