using System.Text;

namespace ExtService.GateWay.API.Helpers
{
    public static class CacheHelpers
    {
        public static string GenerateCacheKey(this string keyInput)
        {
            if (string.IsNullOrEmpty(keyInput))
            {
                throw new ArgumentNullException(nameof(keyInput));
            }

            var keyBuilder = GetKeyBuilder(keyInput);

            return keyBuilder.ToString();
        }

        public static string GenerateCacheKey(this string keyInput, Func<string, string> keyModifier)
        {
            if (string.IsNullOrEmpty(keyInput))
            {
                throw new ArgumentNullException(nameof(keyInput));
            }

            var keyBuilder = GetKeyBuilder(keyInput);

            return keyModifier(keyBuilder.ToString());
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
