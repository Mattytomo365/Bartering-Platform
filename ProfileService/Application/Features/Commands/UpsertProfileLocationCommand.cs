using Application.DTOs;
using MediatR;

namespace Application.Features.Commands;

/// <summary>
/// Input model
/// Application message expressing the intent to upsert a new location into DB
/// Carries data needed to perform the action
/// </summary>

public class UpsertProfileLocationCommand : IRequest<Unit>
{
    public ProfileLocationDto Location { get; init; } = null!;
}
