using ExtService.GateWay.API.Models.Common;
using ExtService.GateWay.API.Models.ServiceModels;
using MediatR;

namespace ExtService.GateWay.API.Models.HandlerModels
{
    public class BillingHandlerModel : IRequest<ServiceResponse<BillingResponse>>
    {
        public Guid IdentificationId { get; set; }
        public string ClientId { get; set; }
        public Guid MethodId { get; set; }
        public DateTime CurrentDate { get; set; }
    }
}
