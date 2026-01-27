using Database;
using Messaging;
using Messaging.WorkEvents;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace WorkloadService;

public static class Extensions
{
    public static IMessageHandler Subscribe(this IMessageHandler mqClient)
    {
        var doWorkQueue = $"{mqClient.ServiceName}.Workload::TradeDoWork";

        mqClient.CreateQueue(queueName: doWorkQueue);

        mqClient.Consume(
            onQueue: doWorkQueue,
            async (DatabaseContext _, ActivitySource _, ILoggerProvider logger, MQEventInfo eventInfo, TradeDoWorkEventBody? eventBody) => {
                await Workload.TradeDoWork(mqClient, eventInfo, eventBody);
            }
        );

        return mqClient;
    }
}
