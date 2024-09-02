using ExtService.GateWay.API.Abstractions.Services;
using ExtService.GateWay.API.Models.Common;
using ExtService.GateWay.API.Models.HandlerModels;
using ExtService.GateWay.API.Models.ServiceModels;
using MediatR;

namespace ExtService.GateWay.API.Handlers
{
    public class GetProxyCacheHandler : IRequestHandler<GetProxyCacheHandlerModel, ServiceResponse<ProxyCache>>
    {
        private readonly ICacheService _cacheService;
        private readonly ILogger<GetProxyCacheHandler> _logger;
        public GetProxyCacheHandler(ICacheService cacheService, ILogger<GetProxyCacheHandler> logger)
        {
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<ServiceResponse<ProxyCache>> Handle(GetProxyCacheHandlerModel request, CancellationToken cancellationToken)
        {
            try
            {
                var cacheResult = await _cacheService.GetCachedDataAsync<ProxyCache>(request.RequestBodyAsKeyInput, key => $"{request.KeyPrefix}:{key}");
                if (!cacheResult.IsSuccess)
                {
                    return new ServiceResponse<ProxyCache>
                    {
                        IsSuccess = false,
                        StatusCode = cacheResult.StatusCode,
                        ErrorMessage = cacheResult.ErrorMessage
                    };
                }

                return new ServiceResponse<ProxyCache>
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Data = cacheResult.Data
                };
            }
            catch (Exception ex)
            {
                string headerMessage = "An unexpected error occurred while getting cache value.";
                _logger.LogError(ex, headerMessage);
                return new ServiceResponse<ProxyCache>
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    ErrorMessage = headerMessage
                };
            }
        }
    }
}
