namespace Application.DTOs;

public class SearchResult
{
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public List<ListingSummary>? Results { get; set; }
}
