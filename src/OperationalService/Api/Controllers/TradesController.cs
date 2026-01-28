using Microsoft.AspNetCore.Mvc;
using OperationalService.Api.Schemas;
using Database;
using OperationalService.BusinessLogic.Objects;
using OperationalService.BusinessLogic.Services;
using System.ComponentModel;
using Messaging;

namespace OperationalService.Api.Controllers;

/// <summary>
/// Controller for managing trades.
/// </summary>
/// <param name="dbContext">The currently connected database context.</param>
[Route("[controller]")]
[ApiController]
public class TradesController(DatabaseContext dbContext, IMessageHandler messageHandler) : ControllerBase
{
    /// <summary>
    /// Get a paginated list of Trade objects.
    /// </summary>
    /// <param name="page">The page to retrieve.</param>
    /// <param name="pageSize">The number of items to retrieve.</param>
    /// <returns>An action result with the paginated list of Trade objects.</returns>
    [HttpGet]
    [Produces<TradesList>]
    public IActionResult GetAll(int page = 1, int pageSize = 10)
    {
        /// 1. Create a Trades service.
        /// 2. Retrieve a paginated collection of Trade objects.
        /// 3. Get the total number of Trade items in the database.
        var tradesService = new TradesService(dbContext: dbContext, messageHandler: messageHandler);
        var trades = tradesService.CollectTrades(page: page, pageSize: pageSize);
        var totalItems = tradesService.CountTrades();

        // Construct the pagination object.
        var pagination = new Pagination()
        {
            PageNumber = page,
            PageSize = pageSize,
            TotalCount = totalItems
        };

        // Construct the response.
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

    /// <summary>
    /// Get a single Trade object.
    /// </summary>
    /// <param name="id">The unique ID of the Trade to retrieve.</param>
    /// <returns>An action result with the retrieved Trade object. If none exists, a NotFound error is returned.</returns>
    [HttpGet("{id}")]
    [Produces<TradeGet>]
    public IActionResult Get([Description("Unique identifier of the Trade.")] string id)
    {
        Guid parsedId;
        
        /// 1. Parse the ID from the request.
        /// 2. If invalid, return a NotFound error.
        try
        {
            parsedId = Guid.Parse(id);
        }
        catch (Exception)
        {
            return NotFound();
        }

        /// 1. Create a Trades service.
        /// 2. Retrieve the Trade with the specified ID.
        var tradesService = new TradesService(dbContext: dbContext, messageHandler: messageHandler);
        var trade = tradesService.FetchTrade(parsedId);

        // If not found, return a NotFound error.
        if (trade is null)
        {
            return NotFound();
        }

        // Construct the response.
        return Ok(
            new TradeGet()
            {
                Id = trade.Id.ToString(),
                Name = trade.Name,
                Status = trade.Status!
            }
        );
    }

    /// <summary>
    /// Create a new Trade object.
    /// </summary>
    /// <param name="tradeCreate">The Trade object to create.</param>
    /// <returns>An action result with the created Trade.</returns>
    [HttpPost]
    [Produces<TradeGet>]
    public IActionResult Create(TradeCreate tradeCreate)
    {
        /// 1. Create a Trades service.
        /// 2. Start a new Trade.
        var tradeObject = new TradeServiceObject() { Name = tradeCreate.Name };
        var tradesService = new TradesService(dbContext: dbContext, messageHandler: messageHandler);
        tradeObject = tradesService.StartTrade(tradeObject);

        // Construct the response.
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
