namespace ExtService.GateWay.API.Abstractions.Services
{
    public interface IProxyContentTransformer
    {
        Task<HttpRequestMessage> TransformAsync(HttpRequestMessage request, object requestContent, CancellationToken cancellationToken);
    }
}
