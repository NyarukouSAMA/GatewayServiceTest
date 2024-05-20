using ExtService.GateWay.API.Abstractions.Factories;
using ExtService.GateWay.API.Models.Common;
using ExtService.GateWay.API.Models.DBModels;
using ExtService.GateWay.API.Models.ServiceRequests;
using MediatR;

namespace ExtService.GateWay.API.Services.ServiceHandlers
{
    public class SearchMethodHandler : IRequestHandler<SearchMethodRequest, ServiceResponse<MethodInfo>>
    {
        private readonly ISearchMethodStrategyFactory _searchMethodStrategyFactory;

        public SearchMethodHandler(ISearchMethodStrategyFactory searchMethodStrategyFactory)
        {
            _searchMethodStrategyFactory = searchMethodStrategyFactory;
        }

        public async Task<ServiceResponse<MethodInfo>> Handle(SearchMethodRequest request, CancellationToken cancellationToken)
        {
            return await _searchMethodStrategyFactory.GetMethodInfoStrategy().GetMethodInfoAsync(request, cancellationToken);
        }
    }
}
