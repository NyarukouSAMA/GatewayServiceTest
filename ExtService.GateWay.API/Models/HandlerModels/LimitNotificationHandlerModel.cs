using ExtService.GateWay.API.Models.Common;
using MediatR;

namespace ExtService.GateWay.API.Models.HandlerModels
{
    public class LimitNotificationHandlerModel : IRequest<ServiceResponse<bool>>
    {
        public Guid BillingId { get; set; }
        public int RequestLimit { get; set; }
        public int RequestCount { get; set; }
        public Guid BillingConfigId { get; set; }
    }
}
