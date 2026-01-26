namespace Messaging.LifecycleEvents;

public sealed record TradeStatusChangeEventBody : MQEventBody
{
    public required string TradeId {  get; init; }
    
    public required string PreviousStatus { get; init; }

    public required string NewStatus { get; init; }
}
