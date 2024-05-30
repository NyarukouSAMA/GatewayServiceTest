using ExtService.GateWay.API.Abstractions.Factories;
using ExtService.GateWay.API.Models.Common;
using ExtService.GateWay.API.Models.HandlerModels;
using ExtService.GateWay.API.Models.HandlerResponses;
using ExtService.GateWay.API.Models.ServiceRequests;
using MediatR;

namespace ExtService.GateWay.API.Handlers
{
    public class IdentificationHandler : IRequestHandler<IdentificationHandlerModel, ServiceResponse<IdentificationHandlerResponse>>
    {
        private readonly IClientIdentificationServiceFactory _clientIdentificationServiceFactory;
        private readonly ISearchMethodServiceFactory _searchMethodServiceFactory;
        private readonly ILogger<IdentificationHandler> _logger;

        public IdentificationHandler(IClientIdentificationServiceFactory clientIdentificationServiceFactory,
            ISearchMethodServiceFactory searchMethodServiceFactory,
            ILogger<IdentificationHandler> logger)
        {
            _clientIdentificationServiceFactory = clientIdentificationServiceFactory;
            _searchMethodServiceFactory = searchMethodServiceFactory;
            _logger = logger;
        }

        public async Task<ServiceResponse<IdentificationHandlerResponse>> Handle(IdentificationHandlerModel request,
            CancellationToken cancellationToken)
        {
            try
            {
                var clientIdentificationService = _clientIdentificationServiceFactory.GetClientIdentificationService();
                var clientIdentificationResult = await clientIdentificationService.IdentifyClientAsync(new ClientIdentificationRequest
                {
                    ClientId = request.ClientId
                }, cancellationToken);

                if (!clientIdentificationResult.IsSuccess)
                {
                    return new ServiceResponse<IdentificationHandlerResponse>
                    {
                        IsSuccess = false,
                        StatusCode = clientIdentificationResult.StatusCode,
                        ErrorMessage = clientIdentificationResult.ErrorMessage
                    };
                }

                var searchMethodService = _searchMethodServiceFactory.GetMethodInfoService();
                var searchMethodResult = await searchMethodService.GetMethodInfoAsync(new SearchMethodRequest
                {
                    MethodName = request.MethodName
                }, cancellationToken);

                if (!searchMethodResult.IsSuccess)
                {
                    return new ServiceResponse<IdentificationHandlerResponse>
                    {
                        IsSuccess = false,
                        StatusCode = searchMethodResult.StatusCode,
                        ErrorMessage = searchMethodResult.ErrorMessage
                    };
                }

                return new ServiceResponse<IdentificationHandlerResponse>
                {
                    IsSuccess = true,
                    Data = new IdentificationHandlerResponse
                    {
                        IdentificationId = clientIdentificationResult.Data.IdentificationId,
                        MethodId = searchMethodResult.Data.MethodId
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request");
                return new ServiceResponse<IdentificationHandlerResponse>
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    ErrorMessage = "An error occurred while processing the request"
                };
            }
        }
    }
}
