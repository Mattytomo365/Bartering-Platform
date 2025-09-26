namespace Application.DTOs;

/// <summary>
/// Response/output DTO - read model returned to client after querying for specified product
/// </summary>

public class SearchResult
{
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public List<ListingSummary>? Results { get; set; }
}
