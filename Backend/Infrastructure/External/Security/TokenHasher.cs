namespace Infrastructure.External.Security;

public static class TokenHasher
{
    public static string HashToken(string token) => Application.Common.TokenHasher.HashToken(token);
}
