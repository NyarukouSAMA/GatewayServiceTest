﻿using ExtService.GateWay.API.Models.Common;

namespace ExtService.GateWay.API.Abstractions.Services
{
    public interface ICacheService
    {
        Task<ServiceResponse<string>> GetCachedDataAsync(string key,
            Func<string, string> keyModifier = null);
        Task<ServiceResponse<TCachedData>> GetCachedDataAsync<TCachedData>(string key,
            Func<string, string> keyModifier = null);
        Task<ServiceResponse<bool>> UpsertDataAsync(string key,
            string data,
            Func<string, string> keyModifier = null,
            TimeSpan ? expiration = null);
        Task<ServiceResponse<bool>> UpsertDataAsync<TCachedData>(string key,
            TCachedData data,
            Func<string, string> keyModifier = null,
            TimeSpan ? expiration = null);
    }
}
