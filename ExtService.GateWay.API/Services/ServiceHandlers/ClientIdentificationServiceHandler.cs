using ExtService.GateWay.API.Abstractions.Factories;
using ExtService.GateWay.API.Helpers;
using ExtService.GateWay.API.Models.Common;
using ExtService.GateWay.API.Models.DBModels;
using ExtService.GateWay.API.Models.ServiceRequests;
using MediatR;

namespace ExtService.GateWay.API.Services.ServiceHandlers
{
    public class ClientIdentificationServiceHandler : IRequestHandler<ClientIdentificationRequest, ServiceResponse<Identification>>
    {
        private readonly IClientIdentificationStrategyFactory _clientIdentificationStrategyFactory;
        private readonly ILogger<ClientIdentificationServiceHandler> _logger;

        public ClientIdentificationServiceHandler(IClientIdentificationStrategyFactory clientIdentificationStrategyFactory,
            ILogger<ClientIdentificationServiceHandler> logger)
        {
            _clientIdentificationStrategyFactory = clientIdentificationStrategyFactory;
            _logger = logger;
        }

        public async Task<ServiceResponse<Identification>> Handle(ClientIdentificationRequest request, CancellationToken cancellationToken)
        {
            try
            {
                return await _clientIdentificationStrategyFactory.GetClientIdentificationStrategy().IdentifyClientAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                string headerMessage = "An error occurred while creating handler logic.";
                _logger.LogError(ex, headerMessage);
                
                return new ServiceResponse<Identification>
                {
                    Data = null,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    IsSuccess = false,
                    ErrorMessage = ex.BuildExceptionMessage(headerMessage)
                };
            }
        }
    }
}
