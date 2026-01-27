using Database;
using Messaging;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics;

namespace OperationalService;

public class App
{
    const string serviceName = "OperationalService";
    const string loggerName = $"{serviceName}.Logger";
    const string tracerName = $"{serviceName}.Tracer";

    public static readonly DateTime StartTime = DateTime.UtcNow;
    public static readonly string LoggerName = loggerName;
    public static readonly string TracerNamer = tracerName;

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder
            .Logging
            .ClearProviders()
            .AddConsole()
            .AddDebug()
            .AddOpenTelemetry(options =>
            {
                options
                    .AddConsoleExporter()
                    .SetResourceBuilder(
                        ResourceBuilder
                            .CreateDefault()
                            .AddService(LoggerName)
                    )
                    .AddProcessor(new ActivityEventLogProcessor());
            });

        builder.Services.AddOpenTelemetry()
            .WithTracing(otelBuilder =>
            {
                otelBuilder
                    .AddSource(serviceName)
                    .AddAspNetCoreInstrumentation()
                    .SetResourceBuilder(
                        ResourceBuilder
                            .CreateDefault()
                            .AddService(TracerNamer)
                    )
                    .AddConsoleExporter()
                    .AddJaegerExporter(conf =>
                    {
                        conf.AgentHost = builder.Configuration.GetSection("Jaeger").GetValue<string>("Host");
                    });
            });

        builder.Services.AddSingleton(new ActivitySource(TracerNamer));
        builder.Services.AddAuthorization();
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddDbContext<DatabaseContext>(options => {
            options.UseNpgsql(builder.Configuration.GetConnectionString("DatabaseConnectionString"));
        });

        var app = builder.Build();
        using var mqClient = MQClient
            .Connect(
                hostName: builder.Configuration.GetSection("RabbitMQ").GetValue<string>("Host")!,
                serviceName: serviceName, 
                serviceProvider: app.Services
            )
            .Subscribe();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}
