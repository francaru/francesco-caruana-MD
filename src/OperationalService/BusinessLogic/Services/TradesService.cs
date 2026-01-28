using Database;
using Database.Entities;
using Messaging;
using Messaging.LifecycleEvents;
using Messaging.WorkEvents;
using OperationalService.BusinessLogic.Objects;
using System.Diagnostics;

namespace OperationalService.BusinessLogic.Services;

/// <summary>
/// A service for the handling of trades business logic.
/// </summary>
/// <param name="dbContext">The currently connected database context.</param>
public class TradesService(DatabaseContext dbContext, IMessageHandler messageHandler)
{
    public void OnStatusChange(ActivitySource activitySource, ILoggerFactory loggerFactory, MQEventInfo eventInfo, TradeStatusChangeEventBody? eventBody)
    {
        /// 1. Start a new tracing activity.
        /// 2. Create a new logger.
        using var _ = activitySource.StartActivity("OnStatusChange");
        var logger = loggerFactory.CreateLogger(eventInfo.QueueName);

        logger.LogInformation($"Received trade status update from: `{eventInfo.Sender.ServiceName}`.");

        /// 1. Parse the Trade ID from the event body.
        /// 2. Retrieve the Trade from the database if it exists.
        var parsedId = Guid.Parse(eventBody!.TradeId);
        var tradeInfo = dbContext
            .TradeInfos
            .SingleOrDefault(ti => ti.TradeId == parsedId);

        if (tradeInfo is not null)
        {
            logger.LogInformation($"Loaded trade information for trade with ID: `{eventBody.TradeId}`.");

            // Update the status of the Trade.
            tradeInfo.Status = eventBody.NewStatus;
            dbContext.TradeInfos.Update(tradeInfo);
            dbContext.SaveChanges();

            logger.LogInformation($"Status changed for trade `{eventBody.TradeId}`: `{eventBody.PreviousStatus}` -> `{eventBody.NewStatus}`.");
        }
    }

    public int CountTrades()
    {
        // Start a new tracing activity.
        using var _ = Activity.Current!.Source.StartActivity("CountTrades");

        // Retrieve the total number of Trade items in the database.
        var totalItems = dbContext.Trades.Count();

        return totalItems;
    }

    public IEnumerable<TradeServiceObject> CollectTrades(int page, int pageSize)
    {
        // Start a new tracing activity.
        using var _ = Activity.Current!.Source.StartActivity("CollectTrades");

        // Retrieve a collection of Trades from the database at an offset.
        var trades = dbContext
            .Trades
            .Take(page * pageSize)
            .Skip((page - 1) * pageSize)
            .Select(t => new TradeServiceObject() { 
                Id = t.Id, 
                Name = t.Name,
                Status = t.TradeInfo!.Status
            })
            .ToList();

        return trades;
    }

    public TradeServiceObject? FetchTrade(Guid id)
    {
        // Start a new tracing activity.
        using var _ = Activity.Current!.Source.StartActivity("FetchTrade");

        // Retrieve the Trade from the database if it exists.
        var trade = dbContext.Trades.SingleOrDefault(t => t.Id == id);

        return (trade is null) ? null : new TradeServiceObject()
        {
            Id = trade.Id,
            Name = trade.Name,
            Status = trade.TradeInfo?.Status
        };
    }

    public TradeServiceObject StartTrade(TradeServiceObject trade)
    {
        // Start a new tracing activity.
        using var _ = Activity.Current!.Source.StartActivity("StartTrade");

        // Create a new Trade database entity.
        var tradeId = Guid.NewGuid();
        var tradeEntity = new TradeEntity()
        {
            Id = tradeId,
            Name = trade.Name,
            TradeInfo = new TradeInfoEntity()
            {
                TradeId = tradeId,
                Status = "starting"
            }
        };

        // Save the Trade in the database.
        dbContext.Trades.Add(tradeEntity);
        dbContext.SaveChanges();

        // Instruct the connected worker to start work on the created Trade.
        messageHandler.Produce(
            toQueues: ["WorkloadService.Workload::TradeDoWork"],
            new TradeDoWorkEventBody() { 
                TradeId = tradeEntity.Id.ToString() 
            }
        );

        return new TradeServiceObject()
        {
            Id = tradeEntity.Id,
            Name = tradeEntity.Name,
            Status = tradeEntity.TradeInfo!.Status
        };
    }
}
