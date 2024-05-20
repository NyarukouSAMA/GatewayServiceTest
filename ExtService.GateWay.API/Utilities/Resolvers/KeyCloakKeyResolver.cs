using ExtService.GateWay.API.Abstractions.Resolvers;
using ExtService.GateWay.API.Constants;
using ExtService.GateWay.API.Models.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;

namespace ExtService.GateWay.API.Utilities.Resolvers
{
    public class KeyCloakKeyResolver : ISigningKeyResolver
    {
        private readonly KeyCloakOptions _keyCloakOptions;
        private readonly ILogger<KeyCloakKeyResolver> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        private string _cachedJWKSUri;
        private ConcurrentDictionary<string, SecurityKey> _keyCache = new ConcurrentDictionary<string, SecurityKey>();

        public KeyCloakKeyResolver(IOptions<KeyCloakOptions> keyCloakOptions,
            ILogger<KeyCloakKeyResolver> logger,
            IHttpClientFactory httpClientFactory)
        {
            _keyCloakOptions = keyCloakOptions.Value;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }
        public IEnumerable<SecurityKey> GetSigningKey(string token, SecurityToken securityToken, string kid, TokenValidationParameters validationParameters)
        {
            if (_keyCache.TryGetValue(kid, out var securityKey))
            {
                return new[] { securityKey };
            }

            try
            {
                RefreshKeySet().Wait();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred when refreshing jwk key set.");
                throw;
            }

            if (_keyCache.TryGetValue(kid, out securityKey))
            {
                return new[] { securityKey };
            }

            string errorMessage = $"The signing key with kid {kid} was not found.";

            _logger.LogError(errorMessage);
            throw new SecurityTokenSignatureKeyNotFoundException(errorMessage);
        }

        private async Task RefreshKeySet()
        {
            if (_cachedJWKSUri == null)
            {
                await LoadMetadata();
            }

            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync(_cachedJWKSUri);

            if (!response.IsSuccessStatusCode)
            {
                string errorResponseMessage = null;
                try
                {
                    errorResponseMessage = await response.Content.ReadAsStringAsync();
                }
                catch { }
                if (string.IsNullOrEmpty(errorResponseMessage))
                {
                    errorResponseMessage = response.ReasonPhrase;
                }

                var errorMessage = string.IsNullOrEmpty(errorResponseMessage)
                    ? $@"Error Getting KeyCloak JWKS.
Status code: {response.StatusCode}"
                    : $@"Error Getting KeyCloak JWKS.
Status code: {response.StatusCode}
ErrorMessage: {errorResponseMessage}";

                throw new HttpRequestException(errorMessage);
            }

            var jwks = await response.Content.ReadAsStringAsync();

            var keys = JsonConvert.DeserializeObject<JsonWebKeySet>(jwks)?.Keys ?? new List<JsonWebKey>();

            if (keys.Count == 0)
            {
                var errorMessage = $"Failed to retrieve keys from JWKS.";

                throw new HttpRequestException(errorMessage);
            }

            foreach (var key in keys)
            {
                if(!_keyCache.TryAdd(key.Kid, key))
                {
                    _keyCache[key.Kid] = key;
                }
            }
        }

        private async Task LoadMetadata()
        {
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync(_keyCloakOptions.KeyCloakRealmMetadataEndpoint);

            if (!response.IsSuccessStatusCode)
            {
                string errorResponseMessage = null;
                try
                {
                    errorResponseMessage = await response.Content.ReadAsStringAsync();
                }
                catch { }
                if (string.IsNullOrEmpty(errorResponseMessage))
                {
                    errorResponseMessage = response.ReasonPhrase;
                }

                var errorMessage = string.IsNullOrEmpty(errorResponseMessage)
                    ? $@"Error Getting KeyCloak metadata.
Status code: {response.StatusCode}"
                    : $@"Error Getting KeyCloak metadata.
Status code: {response.StatusCode}
ErrorMessage: {errorResponseMessage}";

                throw new HttpRequestException(errorMessage);
            }

            var metadata = await response.Content.ReadAsStringAsync();
            var metadataJson = JObject.Parse(metadata);

            var jwksUri = metadataJson?[KeyCloakConstants.MetadataJWKSFieldName]?.Value<string>();

            if (string.IsNullOrEmpty(jwksUri))
            {
                var errorMessage = $"Failed to retrieve JWKS URI from metadata.";

                throw new HttpRequestException(errorMessage);
            }

            _cachedJWKSUri = jwksUri;
        }
    }
}
