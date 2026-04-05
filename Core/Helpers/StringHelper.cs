using System.Globalization;
using System.Text;

namespace Core.Helpers
{
    public static class StringHelper
    {
        /// <summary>
        /// Normalizes a string by converting Turkish characters to English equivalents and converting to uppercase.
        /// Examples: İstanbul -> ISTANBUL, Şehir -> SEHIR, Çağla -> CAGLA
        /// </summary>
        public static string NormalizeTurkish(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            // Turkish character mappings
            var turkishCharMap = new Dictionary<char, char>
            {
                {'ı', 'i'}, {'İ', 'I'}, {'ş', 's'}, {'Ş', 'S'},
                {'ğ', 'g'}, {'Ğ', 'G'}, {'ü', 'u'}, {'Ü', 'U'},
                {'ö', 'o'}, {'Ö', 'O'}, {'ç', 'c'}, {'Ç', 'C'}
            };

            var sb = new StringBuilder(input.Length);
            
            foreach (var c in input)
            {
                if (turkishCharMap.ContainsKey(c))
                {
                    sb.Append(turkishCharMap[c]);
                }
                else
                {
                    sb.Append(c);
                }
            }

            // Convert to uppercase using invariant culture (English rules)
            return sb.ToString().ToUpperInvariant();
        }
    }
}
