using ExtService.GateWay.API.Abstractions.Strategy;
using ExtService.GateWay.API.Abstractions.UnitsOfWork;
using ExtService.GateWay.API.Models.Common;
using ExtService.GateWay.API.Models.DBModels;
using ExtService.GateWay.API.Models.ServiceRequests;

namespace ExtService.GateWay.API.Strategies.SMethodInfo
{
    public class GetMethodByName : IMethodInfoStrategy
    {
        private readonly IDBManager _dbManager;
        private readonly ILogger<GetMethodByName> _logger;

        public GetMethodByName(IDBManager dbManager,
            ILogger<GetMethodByName> logger)
        {
            _dbManager = dbManager;
            _logger = logger;
        }

        public async Task<ServiceResponse<MethodInfo>> GetMethodInfoAsync(SearchMethodRequest searchMethodRequest,
            CancellationToken cancellationToken)
        {
            try
            {
                MethodInfo methodInfo = await _dbManager?.MethodInfoRepository
                    ?.RetrieveAsync(methodInfo => methodInfo.MethodName == searchMethodRequest.MethodName);

                if (methodInfo == null)
                {
                    string errorMessage = $"Method with the given MethodName {searchMethodRequest.MethodName} not found.";
                    _logger.LogError(errorMessage);

                    return new ServiceResponse<MethodInfo>
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        ErrorMessage = errorMessage
                    };
                }

                return new ServiceResponse<MethodInfo>
                {
                    IsSuccess = true,
                    Data = methodInfo
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred when fetching method information.");
                return new ServiceResponse<MethodInfo>
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    ErrorMessage = $"An error occurred when fetching method information. {ex.Message}"
                };
            }
        }
    }
}
