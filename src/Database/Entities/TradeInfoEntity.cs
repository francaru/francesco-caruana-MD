namespace Database.Entities;

/// <summary>
/// A definition for storing additional mutable details for a given Trade object.
/// </summary>
public sealed class TradeInfoEntity
{
    /// <summary>
    /// The unique ID of the Trade being annotated.
    /// </summary>
    public Guid TradeId { get; set; }

    /// <summary>
    /// The Trade instance that links to this TradeInfo object.
    /// </summary>
    public TradeEntity? Trade { get; set; }

    /// <summary>
    /// The status of the Trade (starting | running | complete).
    /// </summary>
    public string? Status { get; set; }
}
