using ExtService.GateWay.API.Models.Common;
using MediatR;

namespace ExtService.GateWay.API.Models.ServiceRequests
{
    public class ProxyRequest : IRequest<ServiceResponse<string>>
    {
        public string Url { get; set; }
        public string Method { get; set; }
        public string Body { get; set; }
    }
}
