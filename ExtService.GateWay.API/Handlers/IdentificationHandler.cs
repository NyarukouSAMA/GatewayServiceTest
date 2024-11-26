using ExtService.GateWay.API.Abstractions.Factories;
using ExtService.GateWay.API.Helpers;
using ExtService.GateWay.API.Models.Common;
using ExtService.GateWay.API.Models.HandlerModels;
using ExtService.GateWay.API.Models.ServiceModels;
using MediatR;
using System.Text;

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

        public async Task<ServiceResponse<IdentificationHandlerResponse>> Handle(IdentificationHandlerModel request, CancellationToken cancellationToken)
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
                    MethodName = request.MethodName,
                    SubMethodName = request.SubMethodName,
                    HttpMethod = request.HttpMethod
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

                StringBuilder requestUriStringBuilder = new StringBuilder();

                requestUriStringBuilder.Append(searchMethodResult.Data.ApiBaseUri);
                if (!string.IsNullOrEmpty(searchMethodResult.Data.ApiPrefix))
                {
                    requestUriStringBuilder.Append($"/{searchMethodResult.Data.ApiPrefix}");
                }
                requestUriStringBuilder.Append($"/{searchMethodResult.Data.MethodPath}");
                if (!string.IsNullOrEmpty(searchMethodResult.Data.SubMethodPath))
                {
                    requestUriStringBuilder.Append($"/{searchMethodResult.Data.SubMethodPath}");
                }

                return new ServiceResponse<IdentificationHandlerResponse>
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Data = new IdentificationHandlerResponse
                    {
                        IdentificationId = clientIdentificationResult.Data.IdentificationId,
                        MethodId = searchMethodResult.Data.MethodId,
                        SystemName = clientIdentificationResult.Data.SystemInfo.SystemName,
                        RequestUri = requestUriStringBuilder.ToString(),
                    }
                };
            }
            catch (Exception ex)
            {
                string headerMessage = "Во время идентификации клиента возникла непредвиденная ошибка.";

                _logger.LogError(ex, headerMessage);
                return new ServiceResponse<IdentificationHandlerResponse>
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    ErrorMessage = ex.BuildExceptionMessage(headerMessage)
                };
            }
        }
    }
}
