namespace Database.Entities;

public sealed class TradeInfoEntity
{
    public Guid TradeId { get; set; }

    public TradeEntity? Trade { get; set; }

    public string? Status { get; set; }
}
