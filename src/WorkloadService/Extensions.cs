using Database;
using Messaging;
using Messaging.WorkEvents;

namespace WorkloadService;

public static class Extensions
{
    public static MQClient Subscribe(this MQClient mqClient)
    {
        var doWorkQueue = $"{mqClient.ServiceName}.Workload::TradeDoWork";

        mqClient.CreateQueue(queueName: doWorkQueue);

        mqClient.Consume(
            onQueue: doWorkQueue,
            (MQEventInfo eventInfo, DatabaseContext _, TradeDoWorkEventBody? eventBody) => {
                Workload.TradeDoWork(mqClient, eventInfo, eventBody);
            }
        );

        return mqClient;
    }
}
