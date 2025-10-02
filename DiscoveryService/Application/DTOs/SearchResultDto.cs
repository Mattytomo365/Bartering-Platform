namespace Application.DTOs;

/// <summary>
/// Output model built from read model projection via repository for a client-friendly output shape
/// Wraps ListingSummary and adds page metadata to form search results
/// </summary>

public class SearchResultDto
{
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public List<ListingSummaryDto>? Results { get; set; }
}
