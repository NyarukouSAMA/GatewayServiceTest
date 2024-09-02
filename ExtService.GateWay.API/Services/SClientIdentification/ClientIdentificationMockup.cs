using ExtService.GateWay.API.Abstractions.Services;
using ExtService.GateWay.API.Models.Common;
using ExtService.GateWay.API.Models.ServiceModels;
using ExtService.GateWay.DBContext.DBModels;

namespace ExtService.GateWay.API.Services.SClientIdentification
{
    public class ClientIdentificationMockup : IClientIdentificationService
    {
        public async Task<ServiceResponse<Identification>> IdentifyClientAsync(ClientIdentificationRequest clientIdentificationRequest,
            CancellationToken cancellationToken)
        {
            return await Task.FromResult(new ServiceResponse<Identification>
            {
                Data = new Identification
                {
                    ClientId = clientIdentificationRequest.ClientId,
                    IdentificationId = Guid.NewGuid(),
                    SystemId = Guid.NewGuid(),
                    EnvName = "Mockup"
                },
                StatusCode = StatusCodes.Status200OK,
                IsSuccess = true
            });
        }
    }
}
