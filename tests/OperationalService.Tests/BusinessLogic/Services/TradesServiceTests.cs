using Database.Entities;
using Messaging;
using Messaging.LifecycleEvents;
using Messaging.WorkEvents;
using Microsoft.Extensions.Logging;
using Moq;
using OperationalService.BusinessLogic.Objects;
using OperationalService.BusinessLogic.Services;
using System.Diagnostics;

namespace OperationalService.Tests.BusinessLogic.Services;

[TestClass]
public class TradesServiceTests
{
    private static ActivitySource? _source;
    private static Activity? _root;

    [ClassInitialize]
    public static void Init(TestContext _)
    {
        TestActivityListener.EnsureInitialized();

        _source = new ActivitySource("Test");
        _root = _source.StartActivity("Root");
    }

    [ClassCleanup]
    public static void Cleanup()
    {
        _root?.Dispose();
        _source?.Dispose();
    }

    [TestMethod]
    public void CountTrades_ReturnsCorrectCount()
    {
        var dbContext = TestDbContextFactory.CreateWithSeed(context =>
        {
            for (int i = 0; i < 5; i++)
            {
                var tradeId = Guid.NewGuid();

                context.Trades.Add(new TradeEntity
                {
                    Id = tradeId,
                    Name = tradeId.ToString(),
                    TradeInfo = new TradeInfoEntity
                    {
                        TradeId = tradeId,
                        Status = new Random().GetItems<string>(["starting", "running", "complete"], 1)[0]
                    }
                });
            }
        });

        var service = new TradesService(dbContext, new Mock<IMessageHandler>().Object);
        var count = service.CountTrades();

        Assert.AreEqual(5, count);
    }

    [TestMethod]
    public void CollectTrades_ReturnsPagedResults()
    {
        var dbContext = TestDbContextFactory.CreateWithSeed(context =>
        {
            for (int i = 0; i < 20; i++)
            {
                var tradeId = Guid.NewGuid();

                context.Trades.Add(new TradeEntity
                {
                    Id = tradeId,
                    Name = tradeId.ToString(),
                    TradeInfo = new TradeInfoEntity
                    {
                        TradeId = tradeId,
                        Status = new Random().GetItems<string>(["starting", "running", "complete"], 1)[0]
                    }
                });
            }
        });

        var service = new TradesService(dbContext, new Mock<IMessageHandler>().Object);
        var results = service.CollectTrades(page: 2, pageSize: 5).ToList();

        Assert.HasCount(5, results);
    }

    [TestMethod]
    public void FetchTrade_NotFound_ReturnsNull()
    {
        var dbContext = TestDbContextFactory.CreateEmpty();
        var service = new TradesService(dbContext, new Mock<IMessageHandler>().Object);

        var result = service.FetchTrade(Guid.NewGuid());

        Assert.IsNull(result);
    }

    [TestMethod]
    public void FetchTrade_Found_ReturnsTrade()
    {
        var dbContext = TestDbContextFactory.CreateWithSeed(context =>
        {
            var tradeId = Guid.NewGuid();

            context.Trades.Add(new TradeEntity
            {
                Id = tradeId,
                Name = tradeId.ToString(),
                TradeInfo = new TradeInfoEntity
                {
                    TradeId = tradeId,
                    Status = new Random().GetItems<string>(["starting", "running", "complete"], 1)[0]
                }
            });
        });

        var trade = dbContext.Trades.First();
        var service = new TradesService(dbContext, new Mock<IMessageHandler>().Object);
        var result = service.FetchTrade(trade.Id);

        Assert.IsNotNull(result);
        Assert.AreEqual(trade.Name, result.Name);
    }

    [TestMethod]
    public void OnStatusChange_UpdatesTradeStatus()
    {
        var dbContext = TestDbContextFactory.CreateWithSeed(context =>
        {
            var tradeId = Guid.NewGuid();

            context.Trades.Add(new TradeEntity
            {
                Id = tradeId,
                Name = tradeId.ToString(),
                TradeInfo = new TradeInfoEntity
                {
                    TradeId = tradeId,
                    Status = "starting"
                }
            });
        });

        var trade = dbContext.TradeInfos.First();
        var service = new TradesService(dbContext, new Mock<IMessageHandler>().Object);
        var eventInfo = new MQEventInfo
        {
            QueueName = "queue",
            Sender = new MQEventRecipient { ServiceName = "Test" }
        };

        var body = new TradeStatusChangeEventBody
        {
            TradeId = trade.TradeId.ToString(),
            PreviousStatus = "starting",
            NewStatus = "complete"
        };

        service.OnStatusChange(
            Activity.Current!.Source,
            new LoggerFactory(),
            eventInfo,
            body
        );

        Assert.AreEqual("complete", trade.Status);
    }
}
