namespace OperationalService.Api.Schemas;

/// <summary>
/// A definition for the retrieval of a single Trade object.
/// </summary>
public sealed class TradeGet
{
    /// <summary>
    /// The unique identified of the Trade.
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// The name of the retrieved Trade.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// The status of the Trade.
    /// </summary>
    public required string Status { get; set; }
}
