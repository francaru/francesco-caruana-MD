namespace OperationalService.Api.Schemas;

/// <summary>
/// A definition for the creation of a Trade object.
/// </summary>
public sealed class TradeCreate
{
    /// <summary>
    /// The name of the Trade being created.
    /// </summary>
    public required string Name { get; set; }
}
