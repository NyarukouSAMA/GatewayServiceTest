using ExtService.GateWay.API.Abstractions.Factories;
using ExtService.GateWay.API.Helpers;
using ExtService.GateWay.API.Models.Common;
using ExtService.GateWay.API.Models.DBModels;
using ExtService.GateWay.API.Models.ServiceRequests;
using MediatR;

namespace ExtService.GateWay.API.Services.ServiceHandlers
{
    public class SearchMethodHandler : IRequestHandler<SearchMethodRequest, ServiceResponse<MethodInfo>>
    {
        private readonly ISearchMethodStrategyFactory _searchMethodStrategyFactory;
        private readonly ILogger<SearchMethodHandler> _logger;

        public SearchMethodHandler(ISearchMethodStrategyFactory searchMethodStrategyFactory,
            ILogger<SearchMethodHandler> logger)
        {
            _searchMethodStrategyFactory = searchMethodStrategyFactory;
            _logger = logger;
        }

        public async Task<ServiceResponse<MethodInfo>> Handle(SearchMethodRequest request, CancellationToken cancellationToken)
        {
            try
            {
                return await _searchMethodStrategyFactory.GetMethodInfoStrategy().GetMethodInfoAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                string headerMessage = "An error occurred while creating handler logic.";
                _logger.LogError(ex, headerMessage);
                
                return new ServiceResponse<MethodInfo>
                {
                    Data = null,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    IsSuccess = false,
                    ErrorMessage = ex.BuildExceptionMessage(headerMessage)
                };
            }
        }
    }
}
