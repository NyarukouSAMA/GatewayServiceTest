using Dapper;
using ExtService.GateWay.API.Abstractions.Services;
using ExtService.GateWay.API.Abstractions.UnitsOfWork;
using ExtService.GateWay.API.Helpers;
using ExtService.GateWay.API.Models.Common;
using ExtService.GateWay.API.Models.ServiceRequests;
using System.Data;

namespace ExtService.GateWay.API.Services.SBilling
{
    public class CheckAndIncrementCounter : IBillingService
    {
        private readonly IDBManager _dbManager;
        private readonly IDbConnection _connection;
        private readonly ILogger<CheckAndIncrementCounter> _logger;

        public CheckAndIncrementCounter(IDBManager dbManager,
            IDbConnection dbConnection,
            ILogger<CheckAndIncrementCounter> logger)
        {
            _dbManager = dbManager;
            _connection = dbConnection;
            _logger = logger;
        }

        public async Task<ServiceResponse<bool>> UpdateBillingRecordAsync(BillingRequest request, CancellationToken cancellationToken)
        {
            try
            {
                if (request == null)
                {
                    string errorMessage = "Запрос не может быть пустым.";
                    _logger.LogError(errorMessage);
                    return new ServiceResponse<bool>
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        ErrorMessage = errorMessage
                    };
                }

                var billingRecord = await _dbManager?.BillingRepository
                    ?.RetrieveAsync(billingRecords => billingRecords.IdentificationId == request.IdentificationId
                    && billingRecords.MethodId == request.MethodId
                    && billingRecords.StartDate <= request.CurrentDate
                    && billingRecords.EndDate >= request.CurrentDate);

                if (billingRecord == null)
                {
                    string errorMessage = $"Биллинговая запись с параметрами clientId: \"{request.ClientId}\" and methodId: \"{request.MethodId}\" не найдена.";
                    _logger.LogError(errorMessage);
                    return new ServiceResponse<bool>
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        ErrorMessage = errorMessage
                    };
                }

                var result = await _connection.ExecuteAsync($@"
UPDATE public.""Billing"" b
SET ""RequestCount"" = ""RequestCount"" + 1
WHERE b.""IdentificationId"" = @IdentificationId
  AND b.""MethodId"" = @MethodId
  AND @CurrentDate BETWEEN b.""StartDate"" AND b.""EndDate""
  AND b.""RequestCount"" < b.""RequestLimit"";
",
                new { request.IdentificationId, request.MethodId, request.CurrentDate });

                if (!(result == 1))
                {
                    string errorMessage = $"Лимит запросов для clientId: \"{request.ClientId}\" по методу methodId: \"{request.MethodId}\" исчерпан.";
                    _logger.LogError(errorMessage);
                    return new ServiceResponse<bool>
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status429TooManyRequests,
                        ErrorMessage = errorMessage
                    };
                }
                else return new ServiceResponse<bool>
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Data = true
                };
            }
            catch (Exception ex)
            {
                string headerMessage = "Во время обработки биллинговой записи возникла непредвиденная ошибка.";

                _logger.LogError(ex, headerMessage);
                return new ServiceResponse<bool>
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    ErrorMessage = ex.BuildExceptionMessage(headerMessage)
                };
            }
        }
    }
}
