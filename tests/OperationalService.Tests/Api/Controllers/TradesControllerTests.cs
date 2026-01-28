using Database.Entities;
using Messaging;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OperationalService.Api.Controllers;
using OperationalService.Api.Schemas;
using System.Diagnostics;

namespace OperationalService.Tests.Api.Controllers;

[TestClass]
public class TradesControllerTests
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
    public void GetAll_ReturnsTradesList()
    {
        var dbContext = TestDbContextFactory.CreateWithSeed(context =>
        {
            for (int i = 0; i < 3; i++)
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

        var controller = new TradesController(dbContext, new Mock<IMessageHandler>().Object);
        var result = controller.GetAll();
        var ok = result as OkObjectResult;

        Assert.IsNotNull(ok);

        var response = ok.Value as TradesList;
        Assert.IsNotNull(response);

        Assert.AreEqual(3, response.Data.Count());
        Assert.AreEqual(1, response.Pagination.PageNumber);
        Assert.AreEqual(10, response.Pagination.PageSize);
        Assert.AreEqual(3, response.Pagination.TotalCount);
    }

    [TestMethod]
    public void Get_InvalidId_ReturnsNotFound()
    {
        var dbContext = TestDbContextFactory.CreateEmpty();
        var controller = new TradesController(dbContext, new Mock<IMessageHandler>().Object);
        var result = controller.Get("not-a-guid");

        Assert.IsInstanceOfType(result, typeof(NotFoundResult));
    }

    [TestMethod]
    public void Get_TradeDoesNotExist_ReturnsNotFound()
    {
        var dbContext = TestDbContextFactory.CreateEmpty();
        var controller = new TradesController(dbContext, new Mock<IMessageHandler>().Object);
        var result = controller.Get(Guid.NewGuid().ToString());

        Assert.IsInstanceOfType(result, typeof(NotFoundResult));
    }

    [TestMethod]
    public void Get_TradeExists_ReturnsTrade()
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

        var controller = new TradesController(dbContext, new Mock<IMessageHandler>().Object);

        var result = controller.Get(trade.Id.ToString());

        var ok = result as OkObjectResult;
        Assert.IsNotNull(ok);

        var dto = ok.Value as TradeGet;
        Assert.IsNotNull(dto);
        Assert.AreEqual(trade.Name, dto.Name);
    }

    [TestMethod]
    public void Create_ReturnsCreatedTrade()
    {
        var dbContext = TestDbContextFactory.CreateEmpty();
        var controller = new TradesController(dbContext, new Mock<IMessageHandler>().Object);
        var result = controller.Create(new TradeCreate { Name = "Test Trade" });

        var created = result as CreatedResult;
        Assert.IsNotNull(created);

        var trade = created.Value as TradeGet;
        Assert.IsNotNull(trade);
        Assert.AreEqual("Test Trade", trade.Name);
    }
}

