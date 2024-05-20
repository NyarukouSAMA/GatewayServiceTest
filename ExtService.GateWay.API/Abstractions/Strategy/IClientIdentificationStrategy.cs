using ExtService.GateWay.API.Models.Common;
using ExtService.GateWay.API.Models.DBModels;
using ExtService.GateWay.API.Models.ServiceRequests;

namespace ExtService.GateWay.API.Abstractions.Strategy
{
    public interface IClientIdentificationStrategy
    {
        Task<ServiceResponse<Identification>> IdentifyClientAsync(ClientIdentificationRequest clientIdentificationRequest,
            CancellationToken cancellationToken);
    }
}
