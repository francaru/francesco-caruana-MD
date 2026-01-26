using Messaging;
using Messaging.LifecycleEvents;
using Messaging.WorkEvents;
using OperationalService.BusinessLogic.Objects;
using Database;
using Database.Entities;

namespace OperationalService.BusinessLogic.Services;

public class TradesService(DatabaseContext dbContext)
{
    public void OnStatusChange(MQClient mqClient, MQEventInfo eventInfo, TradeStatusChangeEventBody? eventBody)
    {
        var parsedId = Guid.Parse(eventBody!.TradeId);

        var tradeInfo = dbContext
            .TradeInfos
            .SingleOrDefault(ti => ti.TradeId == parsedId);

        if (tradeInfo is not null)
        {
            tradeInfo.Status = eventBody.NewStatus;

            dbContext
                .TradeInfos
                .Update(tradeInfo);

            dbContext.SaveChanges();

            Console.WriteLine($"Trade `{eventBody.TradeId}` status changed: `{eventBody.PreviousStatus}` -> `{eventBody.NewStatus}`");
        }
    }

    public int CountTrades()
    {
        var totalItems = dbContext
            .Trades
            .Count();

        return totalItems;
    }

    public IEnumerable<TradeServiceObject> CollectTrades(int page, int pageSize)
    {
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
        var trade = dbContext
            .Trades
            .SingleOrDefault(t => t.Id == id);

        return (trade is null) switch
        {
            true => null,
            false => new TradeServiceObject() { 
                Id = trade.Id, 
                Name = trade.Name,
                Status = trade.TradeInfo?.Status
            },
        };
    }

    public TradeServiceObject StartTrade(TradeServiceObject trade)
    {
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

        dbContext
            .Trades
            .Add(tradeEntity);

        dbContext.SaveChanges();

        MQClient.GetInstance().Produce(
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
