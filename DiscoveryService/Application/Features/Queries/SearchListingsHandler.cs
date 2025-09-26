using Application.DTOs;
using Application.Interfaces;
using MediatR;

namespace Application.Features.Queries
{
    public class SearchListingsHandler : IRequestHandler<SearchListingsQuery, SearchResult>
    {
        private readonly IListingRepository _repo;
        public SearchListingsHandler(IListingRepository repo) => _repo = repo;

        public async Task<SearchResult> Handle(SearchListingsQuery req, CancellationToken ct)
        {
            // No changes needed here; just ensure repository is not used concurrently.
            return await _repo.SearchAsync(req, ct);
        }
    }
}
