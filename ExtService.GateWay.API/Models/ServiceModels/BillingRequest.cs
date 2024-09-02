using ExtService.GateWay.API.Models.Common;
using MediatR;

namespace ExtService.GateWay.API.Models.ServiceModels
{
    public class BillingRequest : IRequest<ServiceResponse<bool>>
    {
        public Guid IdentificationId { get; set; }
        public string ClientId { get; set; }
        public Guid MethodId { get; set; }
        public DateTime CurrentDate { get; set; }
    }
}
