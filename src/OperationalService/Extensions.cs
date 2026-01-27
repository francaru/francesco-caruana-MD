using Database;
using Messaging;
using Messaging.LifecycleEvents;
using OperationalService.BusinessLogic.Services;
using System.Diagnostics;

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
            (DatabaseContext dbContext, ActivitySource activitySource, ILoggerProvider loggerProvider, MQEventInfo eventInfo, TradeStatusChangeEventBody? eventBody) => {
                new TradesService(dbContext).OnStatusChange(mqClient, activitySource, loggerProvider, eventInfo, eventBody);
            }
        );

        return mqClient;
    }
}
