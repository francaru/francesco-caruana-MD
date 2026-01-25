using Messaging;
using Messaging.LifecycleEvents;
using Messaging.WorkEvents;

namespace WorkloadService;

public class Workload
{
    const string serviceName = "WorkloadService";

    void DoWork(MQClient mqClient, MQEventInfo eventInfo, DoWorkEventBody? eventBody)
    {
        Console.WriteLine($"Received instruction by `{eventInfo.Sender.ServiceName}` to start work!!");

        mqClient.Produce(
            toQueues: ["OperationalService.OnStatusChange"],
            new StatusChangeEventBody(previousStatus: "starting", newStatus: "running")
        );

        for (int i = 0; i < 10; i++)
        {
            Thread.Sleep(1000);

            mqClient.Produce(
                toQueues: ["OperationalService.OnProgressChange"],
                new ProgressChangeEventBody(currentStatus: "running", progressPercent: (i + 1.0) / 10.0)
            );
        }

        mqClient.Produce(
            toQueues: ["OperationalService.OnStatusChange"],
            new StatusChangeEventBody(previousStatus: "running", newStatus: "complete")
        );

        Console.WriteLine($"Sent completion confirmation to `{eventInfo.Sender.ServiceName}`!!");
    }

    static void Main(string[] args)
    {
        string doWorkQueue = $"{serviceName}.DoWork";

        using MQClient mqClient = MQClient.GetInstance(serviceName: serviceName);

        mqClient.CreateQueue(queueName: doWorkQueue);
        mqClient.Consume(
            onQueue: doWorkQueue,
            (MQEventInfo eventInfo, DoWorkEventBody? eventBody) => {
                new Workload().DoWork(mqClient, eventInfo, eventBody);
            }
        );

        Console.ReadKey();
    }
}
