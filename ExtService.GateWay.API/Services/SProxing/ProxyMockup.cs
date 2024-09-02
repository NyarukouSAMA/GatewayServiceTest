using ExtService.GateWay.API.Abstractions.Services;
using ExtService.GateWay.API.Models.Common;
using ExtService.GateWay.API.Models.ServiceModels;

namespace ExtService.GateWay.API.Services.SProxing
{
    public class ProxyMockup : IProxingService
    {
        public Task<ServiceResponse<HttpContent>> ExecuteAsync(ProxyRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new ServiceResponse<HttpContent>
            {
                StatusCode = 200,
                IsSuccess = true,
                Data = new StringContent("Mockup response")
            });
        }
    }
}
