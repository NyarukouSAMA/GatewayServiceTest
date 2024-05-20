using Dapper;
using ExtService.GateWay.API.Abstractions.Strategy;
using ExtService.GateWay.API.Abstractions.UnitsOfWork;
using ExtService.GateWay.API.Models.Common;
using ExtService.GateWay.API.Models.ServiceRequests;
using System.Data;

namespace ExtService.GateWay.API.Strategies.SBilling
{
    public class CheckAndIncrementCounter : IBillingStrategy
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

        public async Task<ServiceResponse<bool>> HandleAsync(BillingRequest request, CancellationToken cancellationToken)
        {
            try
            {
                if (request == null)
                {
                    string errorMessage = "Request can't be null.";
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
                    string errorMessage = $"Billing record not found for clientId: {request.ClientId} and methodId: {request.MethodId}";
                    _logger.LogError(errorMessage);
                    return new ServiceResponse<bool>
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        ErrorMessage = errorMessage
                    };
                }

                var result = await _connection.ExecuteAsync($@"
UPDATE Billing
SET RequestCount = RequestCount + 1
WHERE IdentificationId = @IdentificationId
    AND MethodId = @MethodId
    AND @CurrentDate BETWEEN StartDate AND EndDate
    AND RequestCount < RequestLimit
",
                new { request.IdentificationId, request.MethodId, request.CurrentDate });

                if (!(result == 1))
                {
                    string errorMessage = $"Request count exceeded for clientId: {request.ClientId} and methodId: {request.MethodId}";
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
                    Data = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred when handling billing request.");
                return new ServiceResponse<bool>
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    ErrorMessage = $"An error occurred when handling billing request: {ex.Message}"
                };
            }
        }
    }
}
