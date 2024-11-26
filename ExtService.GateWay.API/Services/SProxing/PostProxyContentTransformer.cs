using ExtService.GateWay.API.Abstractions.Services;
using System.Text;

namespace ExtService.GateWay.API.Services.SProxing
{
    public class PostProxyContentTransformer : IProxyContentTransformer
    {
        public Task<HttpRequestMessage> TransformAsync(HttpRequestMessage request, object requestContent, CancellationToken cancellationToken)
        {
            request.Content = new StringContent((string)requestContent, Encoding.UTF8, "application/json");
            return Task.FromResult(request);
        }
    }
}
