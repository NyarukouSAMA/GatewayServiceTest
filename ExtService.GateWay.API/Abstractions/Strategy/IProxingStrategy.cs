using ExtService.GateWay.API.Models.Common;
using ExtService.GateWay.API.Models.ServiceRequests;

namespace ExtService.GateWay.API.Abstractions.Strategy
{
    public interface IProxingStrategy
    {
        Task<ServiceResponse<string>> ExecuteAsync(ProxyRequest request, CancellationToken cancellationToken);
    }
}
