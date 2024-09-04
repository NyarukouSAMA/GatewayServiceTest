using System.Security.Cryptography;
using System.Text;

namespace ExtService.GateWay.API.Helpers
{
    public static class CacheHelpers
    {
        public static string GenerateCacheKey(this string keyInput,
            string keyPrefix = null,
            Func<string, string> keyModifier = null)
        {
            var key = keyInput.TransformString();

            if (keyModifier != null)
            {
                key = keyModifier(key).GetMD5HashFromString();
            }

            if (keyPrefix != null)
            {
                key = $"{keyPrefix}:{key}";
            }

            return key;
        }

        private static string GetMD5HashFromString(this string stringInput)
        {
            using (var md5 = MD5.Create())
            {
                var inputBytes = Encoding.UTF8.GetBytes(stringInput);
                var hashBytes = md5.ComputeHash(inputBytes);

                return BitConverter.ToString(hashBytes).Replace("-", string.Empty).ToLower();
            }
        }

        private static string TransformString(this string keyInput)
        {
            if (string.IsNullOrEmpty(keyInput))
            {
                throw new ArgumentNullException(nameof(keyInput));
            }

            var keyBuilder = GetKeyBuilder(keyInput);

            return keyBuilder.ToString();
        }

        private static StringBuilder GetKeyBuilder(string keyInput)
        {
            var keyBuilder = new StringBuilder();

            foreach (var ch in keyInput)
            {
                if (char.IsLetterOrDigit(ch) || ch == '_')
                {
                    keyBuilder.Append(ch);
                }
            }

            return keyBuilder;
        }
    }
}
