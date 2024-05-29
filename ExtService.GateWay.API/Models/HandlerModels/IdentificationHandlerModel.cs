using ExtService.GateWay.API.Models.Common;
using ExtService.GateWay.API.Models.HandlerResponses;
using MediatR;

namespace ExtService.GateWay.API.Models.HandlerModels
{
    public class IdentificationHandlerModel : IRequest<ServiceResponse<IdentificationHandlerResponse>>
    {
        public string ClientId { get; set; }
        public string MethodName { get; set; } = string.Empty;
    }
}
