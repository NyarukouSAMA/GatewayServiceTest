using ExtService.GateWay.API.Abstractions.Services;
using ExtService.GateWay.API.Models.Common;
using ExtService.GateWay.API.Models.HandlerModels;
using ExtService.GateWay.API.Models.ServiceModels;
using MediatR;

namespace ExtService.GateWay.API.Handlers
{
    public class SetProxyCacheHandler : IRequestHandler<SetProxyCacheHandlerModel, ServiceResponse<bool>>
    {
        private readonly ICacheService _cashService;
        private readonly ILogger<SetProxyCacheHandler> _logger;

        public SetProxyCacheHandler(ICacheService cashService,
            ILogger<SetProxyCacheHandler> logger)
        {
            _cashService = cashService;
            _logger = logger;
        }
        
        public async Task<ServiceResponse<bool>> Handle(SetProxyCacheHandlerModel request, CancellationToken cancellationToken)
        {
            try
            {
                var cacheResult = await _cashService.UpsertDataAsync(request.RequestBodyAsKeyInput,
                    new ProxyCache
                    {
                        RequestBody = request.RequestBodyAsKeyInput,
                        ResponseBody = request.ResponseBody,
                        ContentType = request.ContentType,
                        StatusCode = request.StatusCode
                    },
                    request.KeyPrefix);

                if (!cacheResult.IsSuccess)
                {
                    return new ServiceResponse<bool>
                    {
                        IsSuccess = false,
                        StatusCode = cacheResult.StatusCode,
                        ErrorMessage = cacheResult.ErrorMessage
                    };
                }

                return new ServiceResponse<bool>
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Data = true
                };
            }
            catch (Exception ex)
            {
                string headerMessage = "An unexpected error occurred while setting cache value.";
                _logger.LogError(ex, headerMessage);
                return new ServiceResponse<bool>
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    ErrorMessage = headerMessage
                };
            }
        }
    }
}
