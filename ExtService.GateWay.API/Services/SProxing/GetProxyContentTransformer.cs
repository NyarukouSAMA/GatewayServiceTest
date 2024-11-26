using ExtService.GateWay.API.Abstractions.Services;
using ExtService.GateWay.API.Models.ServiceModels;

namespace ExtService.GateWay.API.Services.SProxing
{
    public class GetProxyContentTransformer : IProxyContentTransformer
    {
        public Task<HttpRequestMessage> TransformAsync(HttpRequestMessage request, object requestContent, CancellationToken cancellationToken)
        {
            var requestParams = (GetRequestParams)requestContent;

            var uriBuilder = new UriBuilder(request.RequestUri);
            uriBuilder.Path += requestParams.PathParam;
            uriBuilder.Query += requestParams.QueryParam;
            request.RequestUri = uriBuilder.Uri;
            return Task.FromResult(request);
        }
    }
}
