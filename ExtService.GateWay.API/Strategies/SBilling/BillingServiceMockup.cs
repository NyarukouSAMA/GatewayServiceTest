using ExtService.GateWay.API.Abstractions.Strategy;
using ExtService.GateWay.API.Models.Common;
using ExtService.GateWay.API.Models.ServiceRequests;

namespace ExtService.GateWay.API.Strategies.SBilling
{
    public class BillingServiceMockup : IBillingStrategy
    {
        public async Task<ServiceResponse<bool>> HandleAsync(BillingRequest request, CancellationToken cancellationToken)
        {
            return await Task.FromResult(new ServiceResponse<bool>
            {
                Data = true,
                StatusCode = StatusCodes.Status200OK,
                IsSuccess = true
            });
        }
    }
}
