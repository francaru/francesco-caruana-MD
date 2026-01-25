using Messaging;
using Messaging.LifecycleEvents;
using Messaging.WorkEvents;

namespace OperationalService.Logic.Services;

public class TradesService
{
    public void OnStatusChange(MQClient mqClient, MQEventInfo eventInfo, StatusChangeEventBody? eventBody)
    {
        if (eventBody is not null)
        {
            Console.WriteLine(
                $"{eventInfo.Sender.ServiceName} work status: `{eventBody.PreviousStatus}` -> `{eventBody.NewStatus}`"
            );
        }
    }

    public void OnProgressChange(MQClient mqClient, MQEventInfo eventInfo, ProgressChangeEventBody? eventBody)
    {
        if (eventBody is not null)
        {
            Console.WriteLine(
                $"{eventInfo.Sender.ServiceName} work progress: {eventBody.ProgressPercent * 100}%"
            );
        }
    }

    public void StartTrade()
    {
        Console.WriteLine("Calling WorkloadService.DoWork");

        MQClient.GetInstance().Produce(
            toQueues: ["WorkloadService.DoWork"],
            new DoWorkEventBody()
        );
    }
}
