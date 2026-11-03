using System.Security.Cryptography;
using System.Text;

namespace Application.Common;

public static class TokenHasher
{
    public static string HashToken(string token)
    {
        if (string.IsNullOrEmpty(token))
            return string.Empty;

        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
