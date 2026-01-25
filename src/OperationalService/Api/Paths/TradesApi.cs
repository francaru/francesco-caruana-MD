using OperationalService.Api.Schemas;

namespace OperationalService.Api.Paths;

public class TradesApi
{
    private const string Route = "/trades";

    static TradeGet Get(HttpContext httpContext)
    {
        return new TradeGet
        {
            Id = 1,
            Name = Guid.NewGuid().ToString(),
        };
    }

    static TradesList Collect(HttpContext httpContext)
    {
        return new TradesList() 
        { 
            Data = Enumerable.Range(1, 5).Select(index => new TradeGet
            {
                Id = index,
                Name = Guid.NewGuid().ToString(),
            }),
            Pagination = new Pagination() { 
                PageNumber = 1,
                PageSize = 10, 
                TotalCount = 10 
            },
        };
    }

    public static void AddRoutes(WebApplication app)
    {
        app.MapGet(Route, Collect).WithName("CollectWeatherForecasts");
        app.MapGet($"{Route}/{"{id}"}", Get).WithName("GetWeatherForecast");
    }
}
