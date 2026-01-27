using Database;
using Messaging;
using Messaging.LifecycleEvents;
using OperationalService.BusinessLogic.Services;
using System.Diagnostics;

namespace OperationalService;

public static class Extensions
{
    public static IMessageHandler Subscribe(this IMessageHandler mqClient)
    {
        var onStatusChangeQueue = $"{mqClient.ServiceName}.BusinessLogic.Service.TradesService::OnStatusChange";

        mqClient.CreateQueue(queueName: onStatusChangeQueue);

        mqClient.Consume(
            onQueue: onStatusChangeQueue,
            (DatabaseContext dbContext, ActivitySource activitySource, ILoggerProvider loggerProvider, MQEventInfo eventInfo, TradeStatusChangeEventBody? eventBody) => {
                new TradesService(dbContext).OnStatusChange(mqClient, activitySource, loggerProvider, eventInfo, eventBody);
            }
        );

        return mqClient;
    }
}
