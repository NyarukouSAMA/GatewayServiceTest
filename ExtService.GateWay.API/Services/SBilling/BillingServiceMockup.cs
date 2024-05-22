using ExtService.GateWay.API.Abstractions.Services;
using ExtService.GateWay.API.Models.Common;
using ExtService.GateWay.API.Models.ServiceRequests;

namespace ExtService.GateWay.API.Services.SBilling
{
    public class BillingServiceMockup : IBillingService
    {
        public async Task<ServiceResponse<bool>> UpdateBillingRecordAsync(BillingRequest request, CancellationToken cancellationToken)
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
