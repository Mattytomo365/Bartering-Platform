using System.Security.Claims;

namespace Web.Auth;

public class GatewayAuthDelegatingHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly string _apiKey;

    public GatewayAuthDelegatingHandler(IHttpContextAccessor httpContextAccessor, IConfiguration config)
    {
        _httpContextAccessor = httpContextAccessor;
        _apiKey = config["DownstreamApiKey"]!;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var user = _httpContextAccessor.HttpContext?.User;
        var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        // Remove original Authorization header
        request.Headers.Remove("Authorization");

        // Inject user identity and API key
        if (!string.IsNullOrEmpty(userId))
        {
            request.Headers.Add("X-User-Id", userId);
        }
        request.Headers.Add("X-Api-Key", _apiKey);

        return base.SendAsync(request, cancellationToken);
    }
}
