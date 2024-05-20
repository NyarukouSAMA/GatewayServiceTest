using ExtService.GateWay.API.Abstractions.Factories;
using ExtService.GateWay.API.Models.Common;
using ExtService.GateWay.API.Models.ServiceRequests;
using MediatR;

namespace ExtService.GateWay.API.Services.ServiceHandlers
{
    public class BillingServiceHandler : IRequestHandler<BillingRequest, ServiceResponse<bool>>
    {
        private readonly IBillingStrategyFactory _billingStrategyFactory;
        public BillingServiceHandler(IBillingStrategyFactory billingStrategyFactory)
        {
            _billingStrategyFactory = billingStrategyFactory;
        }

        public async Task<ServiceResponse<bool>> Handle(BillingRequest request, CancellationToken cancellationToken)
        {
            return await _billingStrategyFactory.GetBillingStrategy().HandleAsync(request, cancellationToken);
        }
    }
}
