using ExtService.GateWay.API.Abstractions.Factories;
using ExtService.GateWay.API.Models.Common;
using ExtService.GateWay.API.Models.HandlerModels;
using ExtService.GateWay.API.Models.ServiceRequests;
using MediatR;

namespace ExtService.GateWay.API.Handlers
{
    public class BillingHandler : IRequestHandler<BillingHandlerModel, ServiceResponse<bool>>
    {
        private readonly IClientIdentificationServiceFactory _clientIdentificationServiceFactory;
        private readonly ISearchMethodServiceFactory _searchMethodServiceFactory;
        private readonly IBillingServiceFactory _billingServiceFactory;
        private readonly ILogger<BillingHandler> _logger;

        public BillingHandler(IClientIdentificationServiceFactory clientIdentificationServiceFactory,
            ISearchMethodServiceFactory searchMethodServiceFactory,
            IBillingServiceFactory billingServiceFactory,
            ILogger<BillingHandler> logger)
        {
            _clientIdentificationServiceFactory = clientIdentificationServiceFactory;
            _searchMethodServiceFactory = searchMethodServiceFactory;
            _billingServiceFactory = billingServiceFactory;
            _logger = logger;
        }

        public async Task<ServiceResponse<bool>> Handle(BillingHandlerModel request, CancellationToken cancellationToken)
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
                    return new ServiceResponse<bool>
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
                    return new ServiceResponse<bool>
                    {
                        IsSuccess = false,
                        StatusCode = searchMethodResult.StatusCode,
                        ErrorMessage = searchMethodResult.ErrorMessage
                    };
                }

                var billingService = _billingServiceFactory.GetBillingService();
                var billingResult = await billingService.UpdateBillingRecordAsync(new BillingRequest
                {
                    IdentificationId = clientIdentificationResult.Data.IdentificationId,
                    ClientId = request.ClientId,
                    MethodId = searchMethodResult.Data.MethodId,
                    CurrentDate = request.CurrentDate
                }, cancellationToken);

                if (!billingResult.IsSuccess)
                {
                    return new ServiceResponse<bool>
                    {
                        IsSuccess = false,
                        StatusCode = billingResult.StatusCode,
                        ErrorMessage = billingResult.ErrorMessage
                    };
                }

                return new ServiceResponse<bool>
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Data = true
                };
            }
            catch (Exception ex)
            {
                string headerMessage = "Во время идентификации и обработки биллинга возникла непредвиденная ошибка.";

                _logger.LogError(ex, ex.Message);
                return new ServiceResponse<bool>
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    ErrorMessage = headerMessage
                };
            }
        }
    }
}
