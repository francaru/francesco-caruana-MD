using Microsoft.EntityFrameworkCore;

namespace Database;

public class Configure
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<DatabaseContext>(options => {
            options.UseNpgsql(builder.Configuration.GetConnectionString("DatabaseConnectionString"));
        });

        var app = builder.Build();

        app.Run();
    }
}