using ExtService.GateWay.API.Abstractions.Factories;
using ExtService.GateWay.API.Models.Common;
using ExtService.GateWay.API.Models.DBModels;
using ExtService.GateWay.API.Models.ServiceRequests;
using MediatR;

namespace ExtService.GateWay.API.Services.ServiceHandlers
{
    public class ClientIdentificationServiceHandler : IRequestHandler<ClientIdentificationRequest, ServiceResponse<Identification>>
    {
        private readonly IClientIdentificationStrategyFactory _clientIdentificationStrategyFactory;

        public ClientIdentificationServiceHandler(IClientIdentificationStrategyFactory clientIdentificationStrategyFactory)
        {
            _clientIdentificationStrategyFactory = clientIdentificationStrategyFactory;
        }

        public async Task<ServiceResponse<Identification>> Handle(ClientIdentificationRequest request, CancellationToken cancellationToken)
        {
            return await _clientIdentificationStrategyFactory.GetClientIdentificationStrategy().IdentifyClientAsync(request, cancellationToken);
        }
    }
}
