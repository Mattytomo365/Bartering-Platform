using Application.DTOs.Requests;
using Application.DTOs.Responses;

namespace Application.Interfaces;

public interface IListingService
{
    Task<Guid> CreateListing(CreateListingRequest req, CancellationToken ct = default);
    Task<Guid> UpdateListing(UpdateListingRequest req, CancellationToken ct = default);
    Task<Guid> DeleteListing(Guid id, CancellationToken ct = default);
    Task<ListingDetailDto> GetListingById(GetListingByIdRequest req, CancellationToken ct = default);
    Task<IEnumerable<ListingDto>> GetUserListings(GetUserListingsRequest req, CancellationToken ct = default);
}