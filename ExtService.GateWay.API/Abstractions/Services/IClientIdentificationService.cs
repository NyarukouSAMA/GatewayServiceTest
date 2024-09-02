using ExtService.GateWay.API.Models.Common;
using ExtService.GateWay.API.Models.ServiceModels;
using ExtService.GateWay.DBContext.DBModels;

namespace ExtService.GateWay.API.Abstractions.Services
{
    public interface IClientIdentificationService
    {
        Task<ServiceResponse<Identification>> IdentifyClientAsync(ClientIdentificationRequest clientIdentificationRequest,
            CancellationToken cancellationToken);
    }
}
