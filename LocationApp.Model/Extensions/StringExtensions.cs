using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

namespace LocationApp.Model.Extensions
{
    public static class StringExtensions
    {
        public static string ToSHA512Hash(this string inputString)
        {
            // Create byte from input string
            SHA512 sha512 = SHA512.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(inputString);
            byte[] hash = sha512.ComputeHash(bytes);

            // Crete hash from bytes
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                result.Append(hash[i].ToString("X2"));
            }

            // Return hash value
            return result.ToString().ToLower();
        }

        public static string GetUserApiKey(this string authorizationHeader)
        {
            var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(AuthenticationHeaderValue.Parse(authorizationHeader).Parameter));
            var parts = credentials.Split(':', 2);

            return parts[0];
        }
    }
}
