using Messaging;
using Messaging.LifecycleEvents;
using OperationalService.Api.Paths;
using OperationalService.Logic.Services;

namespace OperationalService;

public class App
{
    const string serviceName = "OperationalService";

    public static readonly DateTime StartTime = DateTime.UtcNow;

    static void StartLifecylceQueues(string[] args)
    {
        MQClient mqClient = MQClient.GetInstance();

        string onStatusChangeQueue = $"{serviceName}.OnStatusChange";
        string onProgressChangeQueue = $"{serviceName}.OnProgressChange";

        mqClient.CreateQueue(queueName: onStatusChangeQueue);
        mqClient.CreateQueue(queueName: onProgressChangeQueue);

        TradesService tardesService = new();

        mqClient.Consume(
            onQueue: onStatusChangeQueue,
            (MQEventInfo eventInfo, StatusChangeEventBody? eventBody) => {
                tardesService.OnStatusChange(mqClient, eventInfo, eventBody);
            }
        );

        mqClient.Consume(
            onQueue: onProgressChangeQueue,
            (MQEventInfo eventInfo, ProgressChangeEventBody? eventBody) => {
                tardesService.OnProgressChange(mqClient, eventInfo, eventBody);
            }
        );
    }

    static void StartRestAPI(string[] args)
    {
        // Configure a builder for the application.
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        builder.Services.AddAuthorization();
        builder.Services.AddOpenApi();

        // Create an application.
        WebApplication app = builder.Build();
        app.UseAuthorization();

        // Add OpenAPI specification document if running in development mode.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi("openapi.json");
        }

        // Add routes.
        HealthApi.AddRoutes(app);
        TradesApi.AddRoutes(app);

        // Run the application.
        app.Run();
    }

    public static void Main(string[] args)
    {
        using MQClient mqClient = MQClient.GetInstance(serviceName: serviceName);

        StartLifecylceQueues(args);
        StartRestAPI(args);
    }
}
