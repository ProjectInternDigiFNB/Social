using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Application.Common;

public static class SlugHelper
{
    public static string GenerateSlug(string phrase)
    {
        if (string.IsNullOrWhiteSpace(phrase)) return Guid.NewGuid().ToString("N")[..8];

        var str = RemoveDiacritics(phrase).ToLowerInvariant().Trim();
        str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
        str = Regex.Replace(str, @"\s+", "-");
        str = Regex.Replace(str, @"-+", "-").Trim('-');

        return string.IsNullOrWhiteSpace(str) ? Guid.NewGuid().ToString("N")[..8] : str;
    }

    public static string RemoveDiacritics(string text)
    {
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder(capacity: normalizedString.Length);

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder
            .ToString()
            .Normalize(NormalizationForm.FormC)
            .Replace('đ', 'd')
            .Replace('Đ', 'D');
    }
}
