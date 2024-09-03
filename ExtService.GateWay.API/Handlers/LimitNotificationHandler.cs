using ExtService.GateWay.API.Abstractions.Factories;
using ExtService.GateWay.API.Abstractions.Services;
using ExtService.GateWay.API.Models.Common;
using ExtService.GateWay.API.Models.HandlerModels;
using ExtService.GateWay.API.Models.Options;
using ExtService.GateWay.API.Models.QueueMessages;
using MediatR;
using Microsoft.Extensions.Options;

namespace ExtService.GateWay.API.Handlers
{
    public class LimitNotificationHandler : IRequestHandler<LimitNotificationHandlerModel, ServiceResponse<bool>>
    {
        private readonly ILimitCheckServiceFactory _limitCheckServiceFactory;
        private readonly IRabbitMQPublisherService _queueService;
        private readonly IOptions<NotificationExchangeOptions> _notificationExchangeOptions;
        private readonly ILogger<BillingHandler> _logger;

        public LimitNotificationHandler(
            ILimitCheckServiceFactory limitCheckServiceFactory,
            IRabbitMQPublisherService queueService,
            IOptions<NotificationExchangeOptions> notificationExchangeOptions,
            ILogger<BillingHandler> logger)
        {
            _limitCheckServiceFactory = limitCheckServiceFactory;
            _queueService = queueService;
            _notificationExchangeOptions = notificationExchangeOptions;
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

                if (_notificationExchangeOptions.Value == null)
                {
                    string headerMessage = "Не удалось получить настройки обмена для отправки уведомлений о лимите.";
                    _logger.LogError(headerMessage);

                    return new ServiceResponse<bool>
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status500InternalServerError,
                        ErrorMessage = headerMessage
                    };
                }

                var queueResult = await _queueService.PublishToQueue(_notificationExchangeOptions.Value.ExchangeName,
                    _notificationExchangeOptions.Value.RoutingKey,
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
