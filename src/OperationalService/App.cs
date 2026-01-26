using Messaging;
using Messaging.LifecycleEvents;
using Microsoft.EntityFrameworkCore;
using OperationalService.Api.Paths;
using OperationalService.Logic.Services;
using OperationalService.Storage;

namespace OperationalService;

public class App
{
    const string serviceName = "OperationalService";

    public static readonly DateTime StartTime = DateTime.UtcNow;

    static void StartDatabaseContext(WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<RepositoryContext>(options => {
            options.UseNpgsql(builder.Configuration.GetConnectionString("DatabaseConnectionString"));
        });
    }

    static void StartLifecylceQueues()
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

    static void StartRestAPI(WebApplicationBuilder builder)
    {
        builder.Services.AddAuthorization();

        // Add OpenAPI specification to the application.
        builder.Services.AddOpenApi(options =>
        {
            HealthApi.AddOpenAPITransformers(options);
            TradesApi.AddOpenAPITransformers(options);
        });

        // Create an application.
        WebApplication app = builder.Build();
        app.UseAuthorization();

        // Add routes.
        HealthApi.AddRoutes(app);
        TradesApi.AddRoutes(app);

        // Add OpenAPI specification document if running in development mode.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi("openapi.json");
        }

        // Run the application.
        app.Run();
    }

    public static void Main(string[] args)
    {
        // Configure a builder for the application.
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        using MQClient mqClient = MQClient.GetInstance(serviceName: serviceName);

        StartDatabaseContext(builder);
        StartLifecylceQueues();
        StartRestAPI(builder);
    }
}
