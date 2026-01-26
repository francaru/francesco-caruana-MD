using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
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

    static Task GetTrade_OpenAPIOperationTransformer(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        operation.Parameters = [
            new OpenApiParameter()
            {
                Name = "id",
                Description = "The unique identifier of the trade.",
                Required = true,
                Schema = new OpenApiSchema() {
                    Type = JsonSchemaType.Integer,
                    Minimum = "1"
                }
            }
        ];

        return Task.CompletedTask;
    }

    public static void AddOpenAPITransformers(OpenApiOptions options)
    {
        options.AddOperationTransformer(GetTrade_OpenAPIOperationTransformer);
    }

    public static void AddRoutes(WebApplication app)
    {
        var route = app.MapGroup(Route).WithTags("Trades");

        route.MapPost(String.Empty, CreateTrade)
            .WithName("CreateTrade")
            .WithSummary("Create a Trade")
            .WithDescription("Create a new Trade.")
            .Accepts<TradeCreate>(Constants.ApplicationJson)
            .Produces<TradeGet>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status409Conflict);

        route.MapGet(String.Empty, ListTrades)
            .WithName("ListTrades")
            .WithSummary("List Trades")
            .WithDescription("List all Trades.")
            .Produces<TradesList>(StatusCodes.Status200OK);
        
        route.MapGet("/{id:int}", GetTrade)
            .WithName("GetTrade")
            .WithSummary("Get Trade")
            .WithDescription("Get a Trade by ID.")
            .Produces<TradeGet>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
    }
}
