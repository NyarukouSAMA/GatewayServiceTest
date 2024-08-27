using Dapper;
using ExtService.GateWay.API.Abstractions.Services;
using ExtService.GateWay.API.Abstractions.UnitsOfWork;
using ExtService.GateWay.API.Helpers;
using ExtService.GateWay.API.Models.Common;
using ExtService.GateWay.API.Models.ServiceRequests;
using ExtService.GateWay.DBContext.DBModels;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace ExtService.GateWay.API.Services.SBilling
{
    public class BillingService : IBillingService
    {
        private readonly IDBManager _dbManager;
        private readonly IDbConnection _connection;
        private readonly ILogger<BillingService> _logger;

        public BillingService(IDBManager dbManager,
            IDbConnection dbConnection,
            ILogger<BillingService> logger)
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
                    && billingRecords.StartDate.ToUniversalTime() <= request.CurrentDate
                    && billingRecords.EndDate.ToUniversalTime() >= request.CurrentDate);

                if (billingRecord == null)
                {
                    var billingConfig = await _dbManager?.BillingConfigRepository
                        ?.RetrieveAsync(billingConfigs => billingConfigs.IdentificationId == request.IdentificationId
                        && billingConfigs.MethodId == request.MethodId
                        && billingConfigs.StartDate <= request.CurrentDate
                        && billingConfigs.EndDate >= request.CurrentDate);

                    if (billingConfig == null)
                    {
                        string errorMessage = $"Биллинговая запись с параметрами clientId: \"{request.ClientId}\" and methodId: \"{request.MethodId}\" не сконфигурирована.";
                        _logger.LogError(errorMessage);
                        return new ServiceResponse<bool>
                        {
                            IsSuccess = false,
                            StatusCode = StatusCodes.Status404NotFound,
                            ErrorMessage = errorMessage
                        };
                    }

                    if (billingConfig.RequestLimitPerPeriod <= 0)
                    {
                        string errorMessage = $"Лимит запросов для clientId: \"{request.ClientId}\" по методу methodId: \"{request.MethodId}\" не сконфигурирован.";
                        _logger.LogError(errorMessage);
                        return new ServiceResponse<bool>
                        {
                            IsSuccess = false,
                            StatusCode = StatusCodes.Status404NotFound,
                            ErrorMessage = errorMessage
                        };
                    }

                    DateTime curDateStart = request.CurrentDate.Date;
                    DateTime curDateEnd = request.CurrentDate.Date.AddDays(billingConfig.PeriodInDays).AddTicks(-1);

                    DateTime startTime = curDateStart < billingConfig.StartDate ? billingConfig.StartDate : curDateStart;
                    DateTime endTime = curDateEnd > billingConfig.EndDate ? billingConfig.EndDate : curDateEnd;
                    try
                    {
                        var insertResult = await _dbManager?.BillingRepository.InsertAsync(new Billing
                        {
                            BillingId = Guid.NewGuid(),
                            BillingConfigId = billingConfig.BillingConfigId,
                            IdentificationId = request.IdentificationId,
                            MethodId = request.MethodId,
                            StartDate = curDateStart,
                            EndDate = endTime,
                            RequestLimit = billingConfig.RequestLimitPerPeriod,
                            RequestCount = 1
                        });

                        if (insertResult == 1)
                        {
                            return new ServiceResponse<bool>
                            {
                                IsSuccess = true,
                                StatusCode = StatusCodes.Status200OK,
                                Data = true
                            };
                        }
                    }
                    catch(DbUpdateException ex)
                    {
                        if(ex.InnerException is Npgsql.PostgresException pgEx
                            && pgEx.SqlState == "23505"
                            && pgEx.MessageText.Contains("duplicate key value violates unique constraint")
                            && pgEx.MessageText.Contains("BillingConfigId_StartDate_EndDate"))
                        {
                            //This is a workaround for the issue with the unique constraint violation when creating in the same time
                            //Here we just wait for 100 ms and then try to work with the created record instead of creating a new one
                            Task.Delay(100).Wait();
                            _logger.LogWarning("The record already exists. Trying to work with it.");
                        }
                        else
                        {
                            throw;
                        }
                    }
                    catch(InvalidOperationException ex)
                    {
                        if (ex.Message.Contains("the same key value for {'BillingConfigId', 'StartDate', 'EndDate'}"))
                        {
                            //This is a workaround for the issue with the unique constraint violation when creating in the same time
                            //Here we just wait for 100 ms and then try to work with the created record instead of creating a new one
                            Task.Delay(100).Wait();
                            _logger.LogWarning("The record already exists. Trying to work with it.");
                        }
                        else
                        {
                            throw;
                        }
                    }
                    
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
