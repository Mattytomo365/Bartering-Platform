using Application.DTOs;
using Application.Features.Commands;

namespace Application.Interfaces;

public interface IListingService
{
    Task<Guid> CreateListing(CreateListingRequest req, CancellationToken ct = default);
    Task<Guid> UpdateListing(UpdateListingRequest req, CancellationToken ct = default);

    Task<Guid> DeleteListing(Guid id, CancellationToken ct = default);
}