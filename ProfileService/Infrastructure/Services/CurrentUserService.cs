using Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services;

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
