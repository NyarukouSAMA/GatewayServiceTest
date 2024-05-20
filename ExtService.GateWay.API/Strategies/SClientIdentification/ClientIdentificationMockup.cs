using ExtService.GateWay.API.Abstractions.Strategy;
using ExtService.GateWay.API.Models.Common;
using ExtService.GateWay.API.Models.DBModels;
using ExtService.GateWay.API.Models.ServiceRequests;

namespace ExtService.GateWay.API.Strategies.SClientIdentification
{
    public class ClientIdentificationMockup : IClientIdentificationStrategy
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
