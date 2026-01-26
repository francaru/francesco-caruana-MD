using Microsoft.AspNetCore.Mvc;
using OperationalService.Api.Schemas;
using Database;
using OperationalService.BusinessLogic.Objects;
using OperationalService.BusinessLogic.Services;
using System.ComponentModel;

namespace OperationalService.Api.Controllers;

[Route("[controller]")]
[ApiController]
public class TradesController(DatabaseContext dbContext) : ControllerBase
{
    [HttpGet]
    [Produces<TradesList>]
    public IActionResult GetAll(int page = 1, int pageSize = 10)
    {
        var tradesService = new TradesService(dbContext: dbContext);
        var trades = tradesService.CollectTrades(page: page, pageSize: pageSize);
        var totalItems = tradesService.CountTrades();

        var pagination = new Pagination()
        {
            PageNumber = page,
            PageSize = pageSize,
            TotalCount = totalItems
        };

        return Ok(
            new TradesList() { 
                Data = trades.Select(t => new TradeGet() 
                { 
                    Id = t.Id.ToString(), 
                    Name = t.Name,
                    Status = t.Status!
                }), 
                Pagination = pagination 
            }
        );
    }

    [HttpGet("{id}")]
    [Produces<TradeGet>]
    public IActionResult Get([Description("Unique identifier of the Trade.")] string id)
    {
        Guid parsedId;
        
        try
        {
            parsedId = Guid.Parse(id);
        }
        catch (Exception)
        {
            return NotFound();
        }

        var tradesService = new TradesService(dbContext: dbContext);
        var trade = tradesService.FetchTrade(parsedId);

        if (trade is null)
        {
            return NotFound();
        }

        return Ok(
            new TradeGet()
            {
                Id = trade.Id.ToString(),
                Name = trade.Name,
                Status = trade.Status!
            }
        );
    }

    [HttpPost]
    [Produces<TradeGet>]
    public IActionResult Create(TradeCreate tradeCreate)
    {
        var tradeObject = new TradeServiceObject() { Name = tradeCreate.Name };
        var tradesService = new TradesService(dbContext: dbContext);
        
        tradeObject = tradesService.StartTrade(tradeObject);

        return Created(
            $"[controller]/{tradeObject.Id.ToString()}",
            new TradeGet() { 
                Id = tradeObject.Id.ToString(),
                Name = tradeObject.Name,
                Status = tradeObject.Status!
            }
        );
    }
}
