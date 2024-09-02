using ExtService.GateWay.API.Abstractions.Factories;
using ExtService.GateWay.API.Abstractions.Services;
using ExtService.GateWay.API.Constants;
using ExtService.GateWay.API.Models.Common;
using ExtService.GateWay.API.Models.HandlerModels;
using ExtService.GateWay.API.Models.QueueMessages;
using ExtService.GateWay.API.Models.ServiceModels;
using MediatR;

namespace ExtService.GateWay.API.Handlers
{
    public class LimitNotificationHandler : IRequestHandler<LimitNotificationHandlerModel, ServiceResponse<bool>>
    {
        private readonly ILimitCheckServiceFactory _limitCheckServiceFactory;
        private readonly IQueueService _queueService;
        private readonly ILogger<BillingHandler> _logger;

        public LimitNotificationHandler(ILimitCheckServiceFactory limitCheckServiceFactory,
            IQueueService queueService,
            ILogger<BillingHandler> logger)
        {
            _limitCheckServiceFactory = limitCheckServiceFactory;
            _queueService = queueService;
            _logger = logger;
        }

        public async Task<ServiceResponse<bool>> Handle(LimitNotificationHandlerModel request, CancellationToken cancellationToken)
        {
            try
            {
                var limitCheckService = _limitCheckServiceFactory.GetLimitCheckService();
                var checkLimitResult = await limitCheckService.CheckLimitAsync(new LimitNotificationHandlerModel
                {
                    BillingId = request.BillingId,
                    RequestLimit = request.RequestLimit,
                    RequestCount = request.RequestCount,
                    BillingConfigId = request.BillingConfigId
                }, cancellationToken);

                if (!checkLimitResult.IsSuccess
                    || checkLimitResult.Data.SendNotification == false)
                {
                    return new ServiceResponse<bool>
                    {
                        IsSuccess = checkLimitResult.IsSuccess,
                        StatusCode = checkLimitResult.StatusCode,
                        ErrorMessage = checkLimitResult.ErrorMessage,
                        Data = checkLimitResult.Data.SendNotification
                    };
                }

                var queueResult = await _queueService.PublishToQueue(QueueConstants.LimitNotofocationExchange,
                    QueueConstants.LimitNotofocationQueue,
                    new NotificationServiceMassage
                    {
                        RecipientList = checkLimitResult.Data.RecipientList,
                        Message = checkLimitResult.Data.Message,
                        Subject = checkLimitResult.Data.Subject
                    });

                if (!queueResult.IsSuccess)
                {
                    return new ServiceResponse<bool>
                    {
                        IsSuccess = queueResult.IsSuccess,
                        StatusCode = queueResult.StatusCode,
                        ErrorMessage = queueResult.ErrorMessage,
                        Data = false
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
                string headerMessage = "Во время проверки лимита и создания оповещения о лимите возникла непредвиденная ошибка.";

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
