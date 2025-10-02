using Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services;

/// <summary>
/// Provides the current user's identifier based on the HTTP context.
/// </summary>
/// <remarks>This service retrieves the user ID from the HTTP request headers. The user ID is expected to be
/// provided in the "X-User-Id" header. If the header is missing, empty, or invalid, an  <see
/// cref="UnauthorizedAccessException"/> is thrown.</remarks>
public class CurrentUserService : ICurrentUserService
{
    private const string USER_ID_HEADER = "X-User-Id";
    private readonly IHttpContextAccessor _http;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _http = httpContextAccessor;
    }

    public string UserId
    {
        get
        {
            var ctx = _http.HttpContext;
            if (ctx == null ||
                !ctx.Request.Headers.TryGetValue(USER_ID_HEADER, out var values) ||
                string.IsNullOrWhiteSpace(values.FirstOrDefault()))
            {
                throw new UnauthorizedAccessException("Missing user header.");
            }

            var firebaseUserId = values.First();
            if (string.IsNullOrWhiteSpace(firebaseUserId))
                throw new UnauthorizedAccessException("Invalid or empty user ID.");

            return firebaseUserId;
        }
    }
}
