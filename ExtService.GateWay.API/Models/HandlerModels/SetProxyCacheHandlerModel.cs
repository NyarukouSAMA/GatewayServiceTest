using ExtService.GateWay.API.Models.Common;
using MediatR;

namespace ExtService.GateWay.API.Models.HandlerModels
{
    public class SetProxyCacheHandlerModel : IRequest<ServiceResponse<bool>>
    {
        public string KeyPrefix { get; set; }
        public string RequestBodyAsKeyInput { get; set; }
        public string ResponseBody { get; set; }
        public string ContentType { get; set; }
        public int StatusCode { get; set; }
    }
}
