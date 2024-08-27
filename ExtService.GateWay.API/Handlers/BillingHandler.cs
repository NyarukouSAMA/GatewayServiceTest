using ExtService.GateWay.API.Abstractions.Factories;
using ExtService.GateWay.API.Models.Common;
using ExtService.GateWay.API.Models.HandlerModels;
using ExtService.GateWay.API.Models.ServiceRequests;
using MediatR;

namespace ExtService.GateWay.API.Handlers
{
    public class BillingHandler : IRequestHandler<BillingHandlerModel, ServiceResponse<bool>>
    {
        
        private readonly IBillingServiceFactory _billingServiceFactory;
        private readonly ILogger<BillingHandler> _logger;

        public BillingHandler(IBillingServiceFactory billingServiceFactory,
            ILogger<BillingHandler> logger)
        {
            _billingServiceFactory = billingServiceFactory;
            _logger = logger;
        }

        public async Task<ServiceResponse<bool>> Handle(BillingHandlerModel request, CancellationToken cancellationToken)
        {
            try
            {
                var billingService = _billingServiceFactory.GetBillingService();
                var billingResult = await billingService.UpdateBillingRecordAsync(new BillingRequest
                {
                    IdentificationId = request.IdentificationId,
                    ClientId = request.ClientId,
                    MethodId = request.MethodId,
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
