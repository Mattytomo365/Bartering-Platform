using Application.DTOs;
using Application.Features.Queries;
using Application.Interfaces;
using MediatR;

namespace Application.Features.Handlers

/// <summary>
/// Orchestrates projection read use case
/// Accepts SearchListingQuery as input model and delegates to repository
/// Projects SearchListing rows to SearchResult output model wrapping instances of ListingSummary
/// </summary>
{
    public class SearchListingsHandler : IRequestHandler<SearchListingsQuery, SearchResultDto>
    {
        private readonly IListingRepository _repo;
        public SearchListingsHandler(IListingRepository repo) => _repo = repo;

        public async Task<SearchResultDto> Handle(SearchListingsQuery req, CancellationToken ct)
        {
            // No changes needed here; just ensure repository is not used concurrently.
            return await _repo.SearchAsync(req, ct);
        }
    }
}
