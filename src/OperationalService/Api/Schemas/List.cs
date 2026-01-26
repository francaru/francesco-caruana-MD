namespace OperationalService.Api.Schemas;

public abstract class List<T> where T : class
{
    public required IEnumerable<T> Data { get; set; }

    public required Pagination Pagination { get; set; }
}
