using ExtService.GateWay.API.Abstractions.Factories;
using ExtService.GateWay.API.Helpers;
using ExtService.GateWay.API.Models.Common;
using ExtService.GateWay.API.Models.ServiceRequests;
using MediatR;

namespace ExtService.GateWay.API.Services.ServiceHandlers
{
    public class ProxyServiceHandler : IRequestHandler<ProxyRequest, ServiceResponse<string>>
    {
        private readonly IProxingStrategyFactory _proxingStrategyFactory;
        private readonly ILogger<ProxyServiceHandler> _logger;
        public ProxyServiceHandler(IProxingStrategyFactory proxingStrategyFactory,
            ILogger<ProxyServiceHandler> logger)
        {
            _proxingStrategyFactory = proxingStrategyFactory;
            _logger = logger;
        }

        public async Task<ServiceResponse<string>> Handle(ProxyRequest request, CancellationToken cancellationToken)
        {
            try
            {
                return await _proxingStrategyFactory.GetProxingStrategy().ExecuteAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                string headerMessage = "An error occurred while creating handler logic.";
                _logger.LogError(ex, headerMessage);

                return new ServiceResponse<string>
                {
                    Data = string.Empty,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    IsSuccess = false,
                    ErrorMessage = ex.BuildExceptionMessage(headerMessage)
                };
            }
        }
    }
}
