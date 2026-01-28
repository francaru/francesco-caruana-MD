namespace Messaging.WorkEvents;

/// <summary>
/// Describes an event where a Trade is requested to do work.
/// </summary>
public sealed record TradeDoWorkEventBody : MQEventBody
{
    /// <summary>
    /// The unique ID of the Trade that requires work.
    /// </summary>
    public required string TradeId { get; init; }
}
