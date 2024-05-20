using ExtService.GateWay.API.Abstractions.Factories;
using ExtService.GateWay.API.Helpers;
using ExtService.GateWay.API.Models.Common;
using ExtService.GateWay.API.Models.ServiceRequests;
using MediatR;

namespace ExtService.GateWay.API.Services.ServiceHandlers
{
    public class BillingServiceHandler : IRequestHandler<BillingRequest, ServiceResponse<bool>>
    {
        private readonly IBillingStrategyFactory _billingStrategyFactory;
        private readonly ILogger<BillingServiceHandler> _logger;
        public BillingServiceHandler(IBillingStrategyFactory billingStrategyFactory,
            ILogger<BillingServiceHandler> logger)
        {
            _billingStrategyFactory = billingStrategyFactory;
            _logger = logger;
        }

        public async Task<ServiceResponse<bool>> Handle(BillingRequest request, CancellationToken cancellationToken)
        {
            try
            {
                return await _billingStrategyFactory.GetBillingStrategy().HandleAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                string headerMessage = "An error occurred while creating handler logic.";
                _logger.LogError(ex, headerMessage);
                
                return new ServiceResponse<bool>
                {
                    Data = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    IsSuccess = false,
                    ErrorMessage = ex.BuildExceptionMessage(headerMessage)
                };
            }
        }
    }
}
