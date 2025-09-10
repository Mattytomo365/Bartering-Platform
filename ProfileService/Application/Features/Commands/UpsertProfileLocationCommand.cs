using Application.DTOs;
using MediatR;

namespace Application.Features.Commands;

public class UpsertProfileLocationCommand : IRequest<Unit>
{
    public ProfileLocationDto Location { get; init; } = null!;
}
