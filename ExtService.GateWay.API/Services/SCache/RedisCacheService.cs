using ExtService.GateWay.API.Abstractions.Services;
using ExtService.GateWay.API.Helpers;
using ExtService.GateWay.API.Models.Common;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace ExtService.GateWay.API.Services.SCache
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly ILogger<RedisCacheService> _logger;

        public RedisCacheService(IDistributedCache cache,
            ILogger<RedisCacheService> logger)
        {
            _distributedCache = cache;
            _serializerSettings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            };
            _logger = logger;
        }

        public async Task<ServiceResponse<TCachedData>> GetCachedDataAsync<TCachedData>(string keyInput,
            Func<string, string> keyModifyer = null)
        {
            try
            {
                string key = string.Empty;
                if (keyModifyer != null)
                {
                    key = keyInput.GenerateCacheKey(keyModifyer);
                }
                else
                {
                    key = keyInput.GenerateCacheKey();
                }

                var cachedData = await _distributedCache.GetStringAsync(key);
                if (cachedData == null)
                {
                    return new ServiceResponse<TCachedData>
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        ErrorMessage = $"Cached data with not found for key: {keyInput}"
                    };
                }

                return new ServiceResponse<TCachedData>
                {
                    IsSuccess = true,
                    Data = JsonConvert.DeserializeObject<TCachedData>(cachedData, _serializerSettings)
                };
            }
            catch (Exception ex)
            {
                string headerMessage = "Error while getting cached data";

                _logger.LogError(ex, headerMessage);
                return new ServiceResponse<TCachedData>
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    ErrorMessage = headerMessage
                };
            }
        }

        public async Task<ServiceResponse<string>> GetCachedDataAsync(string keyInput,
            Func<string, string> keyModifyer = null)
        {
            try
            {
                string key = string.Empty;
                if (keyModifyer != null)
                {
                    key = keyInput.GenerateCacheKey(keyModifyer);
                }
                else
                {
                    key = keyInput.GenerateCacheKey();
                }

                var cachedData = await _distributedCache.GetStringAsync(key);
                if (cachedData == null)
                {
                    return new ServiceResponse<string>
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        ErrorMessage = $"Cached data with not found for key: {keyInput}"
                    };
                }

                return new ServiceResponse<string>
                {
                    IsSuccess = true,
                    Data = cachedData
                };
            }
            catch (Exception ex)
            {
                string headerMessage = "Error while getting cached data";

                _logger.LogError(ex, headerMessage);
                return new ServiceResponse<string>
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    ErrorMessage = headerMessage
                };
            }
        }

        public async Task<ServiceResponse<bool>> UpsertDataAsync<TCachedData>(string keyInput,
            TCachedData data,
            Func<string, string> keyModifyer = null,
            TimeSpan ? expiration = null)
        {
            try
            {
                string key = string.Empty;

                if (keyModifyer != null)
                {
                    key = keyInput.GenerateCacheKey(keyModifyer);
                }
                else
                {
                    key = keyInput.GenerateCacheKey();
                }

                if (expiration == null)
                {
                    expiration = TimeSpan.FromHours(8);
                }

                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expiration
                };

                var serializedData = JsonConvert.SerializeObject(data, _serializerSettings);

                await _distributedCache.SetStringAsync(key, serializedData, options);

                return new ServiceResponse<bool>
                {
                    IsSuccess = true,
                    Data = true
                };
            }
            catch (Exception ex)
            {
                string headerMessage = "Error while upserting cache data";

                _logger.LogError(ex, headerMessage);
                return new ServiceResponse<bool>
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    ErrorMessage = headerMessage
                };
            }
        }

        public async Task<ServiceResponse<bool>> UpsertDataAsync(string keyInput,
            string data,
            Func<string, string> keyModifyer = null,
            TimeSpan ? expiration = null)
        {
            try
            {
                string key = string.Empty;

                if (keyModifyer != null)
                {
                    key = keyInput.GenerateCacheKey(keyModifyer);
                }
                else
                {
                    key = keyInput.GenerateCacheKey();
                }

                if (expiration == null)
                {
                    expiration = TimeSpan.FromHours(8);
                }

                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expiration
                };

                await _distributedCache.SetStringAsync(key, data, options);

                return new ServiceResponse<bool>
                {
                    IsSuccess = true,
                    Data = true
                };
            }
            catch (Exception ex)
            {
                string headerMessage = "Error while upserting cache data";

                _logger.LogError(ex, headerMessage);
                return new ServiceResponse<bool>
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    ErrorMessage = headerMessage
                };
            }
        }
    }
}
