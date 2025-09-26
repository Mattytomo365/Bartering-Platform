using Application.DTOs;
using MediatR;

namespace Application.Features.Queries;

public class SearchListingsQuery : IRequest<SearchResult>
{
    public string? Q { get; init; }
    public string? Category { get; init; } = "all";
    public int? Page { get; init; } = 1;
    public int? PageSize { get; init; } = 20;
    public double? Lat { get; init; }
    public double? Lng { get; init; }
    public int RadiusKm { get; init; } = 10;
    public string Sort { get; init; } = "relevance";
}
