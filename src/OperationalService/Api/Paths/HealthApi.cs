using Microsoft.AspNetCore.OpenApi;
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

    public static void AddOpenAPITransformers(OpenApiOptions options) { }

    public static void AddRoutes(WebApplication app)
    {
        var route = app.MapGroup(Route).WithTags("Health");

        route.MapGet(String.Empty, CheckHealth)
            .WithName("CheckHealth")
            .WithSummary("Check Health")
            .WithDescription("Check the server's Health.")
            .Produces<Health>(StatusCodes.Status200OK);
    }
}
