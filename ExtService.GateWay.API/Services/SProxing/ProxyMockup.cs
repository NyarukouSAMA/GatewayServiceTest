using ExtService.GateWay.API.Abstractions.Services;
using ExtService.GateWay.API.Models.Common;
using ExtService.GateWay.API.Models.ServiceRequests;

namespace ExtService.GateWay.API.Services.SProxing
{
    public class ProxyMockup : IProxingService
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
