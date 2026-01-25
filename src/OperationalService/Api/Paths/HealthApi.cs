using OperationalService.Api.Schemas;

namespace OperationalService.Api.Paths;

public class HealthApi
{
    private const string Route = "/health";

    static Health Get(HttpContext httpContext)
    {
        return new Health()
        {
            UpTime = DateTime.UtcNow - App.StartTime
        };
    }

    public static void AddRoutes(WebApplication app)
    {
        app.MapGet(Route, Get).WithName("Health");
    }
}
