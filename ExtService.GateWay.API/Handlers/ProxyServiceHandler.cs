using ExtService.GateWay.API.Abstractions.Factories;
using ExtService.GateWay.API.Helpers;
using ExtService.GateWay.API.Models.Common;
using ExtService.GateWay.API.Models.ServiceModels;
using MediatR;

namespace ExtService.GateWay.API.Handlers
{
    public class ProxyServiceHandler : IRequestHandler<ProxyRequest, ServiceResponse<HttpContent>>
    {
        private readonly IProxingServiceFactory _proxingStrategyFactory;
        private readonly ILogger<ProxyServiceHandler> _logger;
        public ProxyServiceHandler(IProxingServiceFactory proxingStrategyFactory,
            ILogger<ProxyServiceHandler> logger)
        {
            _proxingStrategyFactory = proxingStrategyFactory;
            _logger = logger;
        }

        public async Task<ServiceResponse<HttpContent>> Handle(ProxyRequest request, CancellationToken cancellationToken)
        {
            try
            {
                return await _proxingStrategyFactory.GetProxingService().ExecuteAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                string headerMessage = "Во время проксирования запроса возникла непредвиденная ошибка.";
                _logger.LogError(ex, headerMessage);

                return new ServiceResponse<HttpContent>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    IsSuccess = false,
                    ErrorMessage = ex.BuildExceptionMessage(headerMessage)
                };
            }
        }
    }
}
