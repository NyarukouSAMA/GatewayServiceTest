using Dapper;
using ExtService.GateWay.API.Abstractions.Services;
using ExtService.GateWay.API.Abstractions.UnitsOfWork;
using ExtService.GateWay.API.Models.Common;
using ExtService.GateWay.API.Models.HandlerModels;
using ExtService.GateWay.API.Models.ServiceModels;
using ExtService.GateWay.API.Services.SBilling;
using System.Data;

namespace ExtService.GateWay.API.Services.SLimitCheck
{
    public class LimitCheckService : ILimitCheckService
    {
        private readonly IDBManager _dbManager;
        private readonly IDbConnection _connection;
        private readonly ILogger<LimitCheckService> _logger;

        public LimitCheckService(IDBManager dbManager,
            IDbConnection dbConnection,
            ILogger<LimitCheckService> logger)
        {
            _dbManager = dbManager;
            _connection = dbConnection;
            _logger = logger;
        }

        public async Task<ServiceResponse<LimitCheckResponse>> CheckLimitAsync(LimitNotificationHandlerModel limitCheckRequest, CancellationToken cancellationToken)
        {
            try
            {
                var notificationRecord = await _dbManager?.NotificationInfoRepository
                    ?.RetrieveAsync(notificationRecord => notificationRecord.BillingConfigId == limitCheckRequest.BillingConfigId
                    && notificationRecord.BillingId != limitCheckRequest.BillingId
                    && notificationRecord.NotificationLimitPercentage <= limitCheckRequest.RequestCount * 100 / limitCheckRequest.RequestLimit);

                if (notificationRecord == null)
                {
                    return new ServiceResponse<LimitCheckResponse>
                    {
                        IsSuccess = true,
                        StatusCode = 200,
                        Data = new LimitCheckResponse
                        {
                            SendNotification = false
                        }
                    };
                }

                var result = await _dbManager?.NotificationInfoRepository?.UpdateAsync(notificationSetter => notificationSetter
                    .SetProperty(dbRecord => dbRecord.BillingId, limitCheckRequest.BillingId),
                    dbRecord => dbRecord.NotificationId == notificationRecord.NotificationId
                    && dbRecord.BillingId != limitCheckRequest.BillingId);

                if (result == 0) {
                    return new ServiceResponse<LimitCheckResponse>
                    {
                        IsSuccess = true,
                        StatusCode = 200,
                        Data = new LimitCheckResponse
                        {
                            SendNotification = false
                        }
                    };
                }

                return new ServiceResponse<LimitCheckResponse>
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Data = new LimitCheckResponse
                    {
                        SendNotification = true,
                        NotificationId = notificationRecord.NotificationId,
                        CurrentPercentage = limitCheckRequest.RequestCount * 100 / limitCheckRequest.RequestLimit,
                        NotificationLimitPercentage = notificationRecord.NotificationLimitPercentage,
                        RecipientList = notificationRecord.RecipientList,
                        Subject = notificationRecord.Subject,
                        Message = notificationRecord.Message
                    }
                };
            }
            catch (Exception ex)
            {
                string headerMessage = "Во время проверки лимита возникла непредвиденная ошибка.";

                _logger.LogWarning(ex, ex.Message);
                return new ServiceResponse<LimitCheckResponse>
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Data = new LimitCheckResponse
                    {
                        SendNotification = false
                    }
                };
            }
        }
    }
}
