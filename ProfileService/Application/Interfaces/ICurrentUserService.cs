namespace Application.Interfaces;

public interface ICurrentUserService
{
    /// <summary>
    /// The authenticated user’s ID, as conveyed by the gateway’s X-User-Id header.
    /// Throws if missing or invalid.
    /// </summary>
    string UserId { get; }
}