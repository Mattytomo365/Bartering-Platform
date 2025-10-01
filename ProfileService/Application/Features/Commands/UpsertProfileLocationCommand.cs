using Application.DTOs;
using MediatR;

namespace Application.Features.Commands;

public class UpsertProfileLocationCommand : IRequest<Unit>
{
    public required string DisplayName { get; init; }
    public double Latitude { get; init; }
    public double Longitude { get; init; }
}
