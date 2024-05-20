using ExtService.GateWay.API.Models.Common;
using ExtService.GateWay.API.Models.DBModels;
using MediatR;

namespace ExtService.GateWay.API.Models.ServiceRequests
{
    public class ClientIdentificationRequest : IRequest<ServiceResponse<Identification>>
    {
        public string ClientId { get; set; }
    }
}
