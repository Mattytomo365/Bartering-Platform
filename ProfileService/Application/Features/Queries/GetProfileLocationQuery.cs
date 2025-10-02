using Application.DTOs;
using MediatR;

namespace Application.Features.Queries;

/// <summary>
/// Input model
/// Application query message to fetch profile location for a specific user
/// Carries data needed to perform the action
/// </summary>

public class GetProfileLocationQuery : IRequest<ProfileLocationDto?> { }
