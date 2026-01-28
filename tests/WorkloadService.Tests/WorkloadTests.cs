using Messaging;
using Messaging.LifecycleEvents;
using Messaging.WorkEvents;
using Moq;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace WorkloadService.Tests;

[TestClass]
public sealed class WorkloadTests
{
    [TestMethod]
    public async Task TradeDoWork_ProducesRunningAndCompleteEvents()
    {
        var tradeId = Guid.NewGuid().ToString();
        var publisherMock = new Mock<IMessageHandler>();
        var eventBody = new TradeDoWorkEventBody
        {
            TradeId = tradeId
        };

        await Workload.TradeDoWork(
            publisherMock.Object,
            eventInfo: null!,
            eventBody,
            delay: () => Task.CompletedTask
        );

        publisherMock.Verify(m =>
            m.Produce(
                It.IsAny<string[]>(),
                It.Is<TradeStatusChangeEventBody>(b =>
                    b.TradeId == tradeId &&
                    b.PreviousStatus == "starting" &&
                    b.NewStatus == "running"
                )),
            Times.Once);

        publisherMock.Verify(m =>
            m.Produce(
                It.IsAny<string[]>(),
                It.Is<TradeStatusChangeEventBody>(b =>
                    b.TradeId == tradeId &&
                    b.PreviousStatus == "running" &&
                    b.NewStatus == "complete"
                )),
            Times.Once);
    }

    [TestMethod]
    public void TradeStatusChangeEventBody_RoundtripSerialization()
    {
        var payload = new TradeStatusChangeEventBody
        {
            TradeId = Guid.NewGuid().ToString(),
            PreviousStatus = "running",
            NewStatus = "complete"
        };

        var mqEvent = new MQEvent<TradeStatusChangeEventBody>
        {
            Body = payload,
            Recipient = new MQEventRecipient
            {
                ServiceName = "WorkloadService"
            }
        };

        var json = JsonSerializer.Serialize(mqEvent);
        var deserialized = JsonSerializer.Deserialize<MQEvent<TradeStatusChangeEventBody>>(json);

        Assert.IsNotNull(deserialized);
        Assert.IsNotNull(deserialized.Body);

        Assert.AreEqual(payload.TradeId, deserialized.Body.TradeId);
        Assert.AreEqual(payload.PreviousStatus, deserialized.Body.PreviousStatus);
        Assert.AreEqual(payload.NewStatus, deserialized.Body.NewStatus);
    }

    [TestMethod]
    public void TradeStatusChangeEventBody_ContainsExpectedProperties()
    {
        var payload = new TradeStatusChangeEventBody
        {
            TradeId = "test",
            PreviousStatus = "starting",
            NewStatus = "running"
        };

        var json = JsonSerializer.Serialize(payload);
        var node = JsonNode.Parse(json)!;

        Assert.IsNotNull(node["TradeId"]);
        Assert.IsNotNull(node["PreviousStatus"]);
        Assert.IsNotNull(node["NewStatus"]);
    }

}
