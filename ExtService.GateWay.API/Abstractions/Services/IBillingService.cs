using ExtService.GateWay.API.Models.Common;
using ExtService.GateWay.API.Models.ServiceRequests;

namespace ExtService.GateWay.API.Abstractions.Services
{
    public interface IBillingService
    {
        Task<ServiceResponse<bool>> UpdateBillingRecordAsync(BillingRequest request, CancellationToken cancellationToken);
    }
}
