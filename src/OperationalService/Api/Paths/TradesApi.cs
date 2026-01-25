using OperationalService.Api.Schemas;

namespace OperationalService.Api.Paths;

public class TradesApi
{
    private const string Route = "/trades";

    static IResult CreateTrade(TradeCreate tradeCreate)
    {
        return Results.Ok(
            new TradeGet()
            {
                Id = 2,
                Name = tradeCreate.Name
            }
        );
    }

    static IResult ListTrades()
    {
        return Results.Ok(
            new TradesList()
            {
                Data = Enumerable.Range(1, 5).Select(index => new TradeGet
                {
                    Id = index,
                    Name = Guid.NewGuid().ToString(),
                }),
                Pagination = new Pagination()
                {
                    PageNumber = 1,
                    PageSize = 10,
                    TotalCount = 10
                },
            }
        );
    }

    static IResult GetTrade(int id)
    {
        return Results.Ok(
            new TradeGet
            {
                Id = id,
                Name = Guid.NewGuid().ToString(),
            }
        );
    }

    public static void AddRoutes(WebApplication app)
    {
        app.MapPost(Route, CreateTrade).WithName("CreateTrade");

        app.MapGet(Route, ListTrades).WithName("ListTrades");
        
        app.MapGet($"{Route}/{{id:int}}", GetTrade).WithName("GetTrade");
    }
}
