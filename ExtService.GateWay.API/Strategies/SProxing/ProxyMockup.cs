using ExtService.GateWay.API.Abstractions.Strategy;
using ExtService.GateWay.API.Models.Common;
using ExtService.GateWay.API.Models.ServiceRequests;

namespace ExtService.GateWay.API.Strategies.SProxing
{
    public class ProxyMockup : IProxingStrategy
    {
        public async Task<ServiceResponse<string>> ExecuteAsync(ProxyRequest request, CancellationToken cancellationToken)
        {
            return await Task.FromResult(new ServiceResponse<string>
            {
                Data = "Mockup response",
                StatusCode = StatusCodes.Status200OK,
                IsSuccess = true
            });
        }
    }
}
