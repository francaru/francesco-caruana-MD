using Database;
using Messaging;
using Messaging.LifecycleEvents;
using OperationalService.BusinessLogic.Services;

namespace OperationalService;

public static class Extensions
{
    public static MQClient Subscribe(this MQClient mqClient)
    {
        var onStatusChangeQueue = $"{mqClient.ServiceName}.BusinessLogic.Service.TradesService::OnStatusChange";
        var onProgressChangeQueue = $"{mqClient.ServiceName}.BusinessLogic.Service.TradesService::OnProgressChange";

        mqClient.CreateQueue(queueName: onStatusChangeQueue);
        mqClient.CreateQueue(queueName: onProgressChangeQueue);

        mqClient.Consume(
            onQueue: onStatusChangeQueue,
            (MQEventInfo eventInfo, DatabaseContext dbContext, TradeStatusChangeEventBody? eventBody) => {
                new TradesService(dbContext).OnStatusChange(mqClient, eventInfo, eventBody);
            }
        );

        return mqClient;
    }
}
