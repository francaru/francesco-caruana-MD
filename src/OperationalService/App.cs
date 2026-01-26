using Messaging;
using Microsoft.EntityFrameworkCore;
using Database;

namespace OperationalService;

public class App
{
    const string serviceName = "OperationalService";

    public static readonly DateTime StartTime = DateTime.UtcNow;

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddAuthorization();
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddDbContext<DatabaseContext>(options => {
            options.UseNpgsql(builder.Configuration.GetConnectionString("DatabaseConnectionString"));
        });

        var app = builder.Build();
        using var mqClient = MQClient
            .Connect(serviceName: serviceName, serviceProvider: app.Services)
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
