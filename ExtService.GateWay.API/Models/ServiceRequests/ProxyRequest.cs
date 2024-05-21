using ExtService.GateWay.API.Models.Common;
using MediatR;

namespace ExtService.GateWay.API.Models.ServiceRequests
{
    public class ProxyRequest : IRequest<ServiceResponse<HttpResponseMessage>>
    {
        public HttpMethod Method { get; set; }
        public string RequestMethodName { get; set; }
        public string RequestPath { get; set; }
        public string Body { get; set; }
    }
}
