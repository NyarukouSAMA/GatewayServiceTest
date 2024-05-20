using ExtService.GateWay.API.Abstractions.Factories;
using ExtService.GateWay.API.Models.Common;
using ExtService.GateWay.API.Models.ServiceRequests;
using MediatR;

namespace ExtService.GateWay.API.Services.ServiceHandlers
{
    public class ProxyServiceHandler : IRequestHandler<ProxyRequest, ServiceResponse<string>>
    {
        private readonly IProxingStrategyFactory _proxingStrategyFactory;

        public ProxyServiceHandler(IProxingStrategyFactory proxingStrategyFactory)
        {
            _proxingStrategyFactory = proxingStrategyFactory;
        }

        public async Task<ServiceResponse<string>> Handle(ProxyRequest request, CancellationToken cancellationToken)
        {
            return await _proxingStrategyFactory.GetProxingStrategy().ExecuteAsync(request, cancellationToken);
        }
    }
}
