namespace Database.Entities;

/// <summary>
/// A definition for storing the details of an immutable Trade object.
/// </summary>
public sealed class TradeEntity
{
    /// <summary>
    /// The unique ID of the Trade.
    /// </summary>
    public required Guid Id { get; set; }

    /// <summary>
    /// The name of the Trade.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// The TradeInfo instance that links to this Trade object.
    /// </summary>
    public TradeInfoEntity? TradeInfo { get; set; }
}
