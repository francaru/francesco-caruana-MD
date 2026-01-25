using OperationalService.Api.Schemas;

namespace OperationalService.Api.Paths;

public class HealthApi
{
    private const string Route = "/health";

    static IResult CheckHealth()
    {
        return Results.Ok(
            new Health()
            {
                UpTime = DateTime.UtcNow - App.StartTime
            }
        );
    }

    public static void AddRoutes(WebApplication app)
    {
        app.MapGet(Route, CheckHealth).WithName("CheckHealth");
    }
}
