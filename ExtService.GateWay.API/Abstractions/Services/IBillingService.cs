using ExtService.GateWay.API.Models.Common;
using ExtService.GateWay.API.Models.ServiceModels;

namespace ExtService.GateWay.API.Abstractions.Services
{
    public interface IBillingService
    {
        Task<ServiceResponse<BillingResponse>> UpdateBillingRecordAsync(BillingRequest request, CancellationToken cancellationToken);
    }
}
