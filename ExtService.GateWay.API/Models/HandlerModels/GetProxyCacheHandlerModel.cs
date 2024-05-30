using ExtService.GateWay.API.Models.Common;
using ExtService.GateWay.API.Models.HandlerResponses;
using MediatR;

namespace ExtService.GateWay.API.Models.HandlerModels
{
    public class GetProxyCacheHandlerModel : IRequest<ServiceResponse<ProxyCache>>
    {
        public string KeyPrefix { get; set; }
        public string KeyInput { get; set; }
    }
}
