namespace Messaging.WorkEvents;

public sealed record TradeDoWorkEventBody : MQEventBody
{
    public required string TradeId { get; init; }
}
