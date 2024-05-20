using ExtService.GateWay.API.Models.Common;
using ExtService.GateWay.API.Models.DBModels;
using ExtService.GateWay.API.Models.ServiceRequests;

namespace ExtService.GateWay.API.Abstractions.Strategy
{
    public interface IMethodInfoStrategy
    {
        Task<ServiceResponse<MethodInfo>> GetMethodInfoAsync(SearchMethodRequest searchMethodRequest, CancellationToken cancellationToken);
    }
}
