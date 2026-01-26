namespace OperationalService.BusinessLogic.Objects;

public class TradeServiceObject
{   
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public string? Status { get; set; }
}
