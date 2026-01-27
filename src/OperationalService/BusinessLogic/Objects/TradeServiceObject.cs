namespace OperationalService.BusinessLogic.Objects;

public sealed class TradeServiceObject
{   
    /// <summary>
    /// The unique identified of the Trade.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The name of the Trade.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// The status of the Trade.
    /// </summary>
    public string? Status { get; set; }
}
