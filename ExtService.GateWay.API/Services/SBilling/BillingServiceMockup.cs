using ExtService.GateWay.API.Abstractions.Services;
using ExtService.GateWay.API.Models.Common;
using ExtService.GateWay.API.Models.ServiceModels;

namespace ExtService.GateWay.API.Services.SBilling
{
    public class BillingServiceMockup : IBillingService
    {
        public async Task<ServiceResponse<BillingResponse>> UpdateBillingRecordAsync(BillingRequest request, CancellationToken cancellationToken)
        {
            return await Task.FromResult(new ServiceResponse<BillingResponse>
            {
                StatusCode = StatusCodes.Status200OK,
                IsSuccess = true,
                Data = new BillingResponse
                {
                    BillingId = Guid.NewGuid(),
                    BillingConfigId = Guid.NewGuid(),
                    RequestCount = 1,
                    RequestLimit = 100,
                }
            });
        }
    }
}
