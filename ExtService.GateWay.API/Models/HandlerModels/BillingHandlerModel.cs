using ExtService.GateWay.API.Models.Common;
using MediatR;

namespace ExtService.GateWay.API.Models.HandlerModels
{
    public class BillingHandlerModel : IRequest<ServiceResponse<bool>>
    {
        public string ClientId { get; set; }
        public string MethodName { get; set; } = string.Empty;
        public DateTime CurrentDate { get; set; }
    }
}
