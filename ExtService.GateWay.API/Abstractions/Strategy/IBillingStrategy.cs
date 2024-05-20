using ExtService.GateWay.API.Models.Common;
using ExtService.GateWay.API.Models.ServiceRequests;

namespace ExtService.GateWay.API.Abstractions.Strategy
{
    public interface IBillingStrategy
    {
        Task<ServiceResponse<bool>> HandleAsync(BillingRequest request, CancellationToken cancellationToken);
    }
}
