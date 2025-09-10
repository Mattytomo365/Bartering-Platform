using Application.DTOs;
using MediatR;

namespace Application.Features.Queries;

public class GetProfileLocationQuery : IRequest<ProfileLocationDto?> { }
