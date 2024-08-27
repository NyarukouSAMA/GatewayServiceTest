using ExtService.GateWay.API.Models.Common;
using MediatR;

namespace ExtService.GateWay.API.Models.HandlerModels
{
    public class BillingHandlerModel : IRequest<ServiceResponse<bool>>
    {
        public Guid IdentificationId { get; set; }
        public string ClientId { get; set; }
        public Guid MethodId { get; set; }
        public DateTime CurrentDate { get; set; }
    }
}
