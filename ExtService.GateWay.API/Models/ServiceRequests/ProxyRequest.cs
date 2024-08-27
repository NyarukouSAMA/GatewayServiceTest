using ExtService.GateWay.API.Models.Common;
using ExtService.GateWay.DBContext.DBModels;
using MediatR;

namespace ExtService.GateWay.API.Models.ServiceRequests
{
    public class ProxyRequest : IRequest<ServiceResponse<HttpContent>>
    {
        public HttpMethod Method { get; set; }
        public int ApiTimeout { get; set; }
        public string RequestUri { get; set; }
        public List<MethodHeaders> MethodHeaders { get; set; }
        public string Body { get; set; }
    }
}
