namespace Application.Interfaces;

public record GoogleUserPayload(
    string Subject,
    string Email,
    bool EmailVerified,
    string? Name,
    string? Picture);

public interface IGoogleAuthVerifier
{
    Task<GoogleUserPayload> VerifyIdTokenAsync(string idToken, CancellationToken cancellationToken = default);
}
