namespace Messaging.LifecycleEvents;

/// <summary>
/// Describes an event where the status of a Trade has changed.
/// </summary>
public sealed record TradeStatusChangeEventBody : MQEventBody
{
    /// <summary>
    /// The unique ID of the Trade for which the status has changed.
    /// </summary>
    public required string TradeId {  get; init; }
    
    /// <summary>
    /// The previous status of the Trade.
    /// </summary>
    public required string PreviousStatus { get; init; }

    /// <summary>
    /// The new status of the Trade.
    /// </summary>
    public required string NewStatus { get; init; }
}
