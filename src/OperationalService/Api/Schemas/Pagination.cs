namespace OperationalService.Api.Schemas;

public class Pagination
{
    public required int PageNumber { get; set; }

    public required int PageSize { get; set; }

    public required int TotalCount { get; set; }
}
