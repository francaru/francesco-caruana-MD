namespace Database.Entities;

public sealed class TradeEntity
{
    public required Guid Id { get; set; }

    public required string Name { get; set; }

    public TradeInfoEntity? TradeInfo { get; set; }
}
