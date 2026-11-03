using Application.Interfaces;
using Application.Settings;
using Domain.Exceptions;
using Google.Apis.Auth;
using Microsoft.Extensions.Options;

namespace Infrastructure.External.Security;

public sealed class GoogleAuthVerifier : IGoogleAuthVerifier
{
    private readonly GoogleSettings _googleSettings;

    public GoogleAuthVerifier(IOptions<GoogleSettings> googleSettings)
    {
        _googleSettings = googleSettings.Value;
    }

    public async Task<GoogleUserPayload> VerifyIdTokenAsync(string idToken, CancellationToken cancellationToken = default)
    {
        GoogleJsonWebSignature.Payload payload;
        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { _googleSettings.ClientId }
            };
            payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
        }
        catch (Exception)
        {
            throw new ValidationAppException("IdToken", "Mã xác thực Google (ID Token) không hợp lệ hoặc đã hết hạn.");
        }

        return new GoogleUserPayload(
            payload.Subject,
            payload.Email ?? string.Empty,
            payload.EmailVerified,
            payload.Name,
            payload.Picture);
    }
}
