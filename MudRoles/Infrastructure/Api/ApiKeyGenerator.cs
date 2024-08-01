using System.Security.Cryptography;
using System.Text;

namespace MudRoles.Infrastructure.Api
{
    public class ApiKeyGenerator
    {
        /// <summary>
        /// Generates a new API key.
        /// </summary>
        /// <returns>A new API key as a string.</returns>
        public static string GenerateApiKey()
        {
            try
            {
                var randomBytes = GenerateRandomBytes(32);
                var base32String = EncodeToBase32(randomBytes);
                return FormatApiKey(base32String);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                throw new InvalidOperationException("An error occurred while generating the API key.", ex);
            }
        }

        /// <summary>
        /// Generates cryptographically strong random bytes.
        /// </summary>
        /// <param name="length">The length of the byte array.</param>
        /// <returns>A byte array filled with random values.</returns>
        private static byte[] GenerateRandomBytes(int length)
        {
            var randomBytes = new byte[length];
            RandomNumberGenerator.Fill(randomBytes);
            return randomBytes;
        }

        /// <summary>
        /// Encodes a byte array to a Base32 string.
        /// </summary>
        /// <param name="bytes">The byte array to encode.</param>
        /// <returns>A Base32 encoded string.</returns>
        private static string EncodeToBase32(byte[] bytes)
        {
            const string base32Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
            StringBuilder result = new StringBuilder((bytes.Length + 4) / 5 * 8);

            for (int i = 0; i < bytes.Length; i += 5)
            {
                byte[] buffer = new byte[8];
                int bufferLength = Math.Min(5, bytes.Length - i);
                Array.Copy(bytes, i, buffer, 0, bufferLength);

                result.Append(base32Chars[(buffer[0] & 0xF8) >> 3]);
                result.Append(base32Chars[((buffer[0] & 0x07) << 2) | ((buffer[1] & 0xC0) >> 6)]);
                result.Append(base32Chars[(buffer[1] & 0x3E) >> 1]);
                result.Append(base32Chars[((buffer[1] & 0x01) << 4) | ((buffer[2] & 0xF0) >> 4)]);
                result.Append(base32Chars[((buffer[2] & 0x0F) << 1) | ((buffer[3] & 0x80) >> 7)]);
                result.Append(base32Chars[(buffer[3] & 0x7C) >> 2]);
                result.Append(base32Chars[((buffer[3] & 0x03) << 3) | ((buffer[4] & 0xE0) >> 5)]);
                result.Append(base32Chars[buffer[4] & 0x1F]);
            }

            return result.ToString().TrimEnd('=');
        }

        /// <summary>
        /// Formats the Base32 string with hyphens for readability.
        /// </summary>
        /// <param name="base32String">The Base32 encoded string.</param>
        /// <returns>A formatted API key string.</returns>
        private static string FormatApiKey(string base32String)
        {
            return string.Join("-", Enumerable.Range(0, base32String.Length / 4).Select(i => base32String.Substring(i * 4, 4)));
        }
    }
}
