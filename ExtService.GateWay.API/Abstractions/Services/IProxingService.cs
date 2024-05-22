using ExtService.GateWay.API.Models.Common;
using ExtService.GateWay.API.Models.ServiceRequests;

namespace ExtService.GateWay.API.Abstractions.Services
{
    public interface IProxingService
    {
        Task<ServiceResponse<string>> ExecuteAsync(ProxyRequest request, CancellationToken cancellationToken);
    }
}
