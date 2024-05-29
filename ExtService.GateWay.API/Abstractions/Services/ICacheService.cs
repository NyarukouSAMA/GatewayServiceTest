using ExtService.GateWay.API.Models.Common;

namespace ExtService.GateWay.API.Abstractions.Services
{
    public interface ICacheService
    {
        Task<ServiceResponse<string>> GetCachedDataAsync(string key);
        Task<ServiceResponse<TCachedData>> GetCachedDataAsync<TCachedData>(string key);
        Task<ServiceResponse<bool>> UpsertDataAsync(string key, string data, TimeSpan? expiration = null);
        Task<ServiceResponse<bool>> UpsertDataAsync<TCachedData>(string key, TCachedData data, TimeSpan? expiration = null);
    }
}
