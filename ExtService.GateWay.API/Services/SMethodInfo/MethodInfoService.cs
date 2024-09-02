using ExtService.GateWay.API.Abstractions.Services;
using ExtService.GateWay.API.Abstractions.UnitsOfWork;
using ExtService.GateWay.API.Helpers;
using ExtService.GateWay.API.Models.Common;
using ExtService.GateWay.API.Models.DTO;
using ExtService.GateWay.API.Models.ServiceModels;
using ExtService.GateWay.DBContext.DBModels;

namespace ExtService.GateWay.API.Services.SMethodInfo
{
    public class MethodInfoService : IMethodInfoService
    {
        private readonly IDBManager _dbManager;
        private readonly ILogger<MethodInfoService> _logger;

        public MethodInfoService(IDBManager dbManager,
            ILogger<MethodInfoService> logger)
        {
            _dbManager = dbManager;
            _logger = logger;
        }

        public async Task<ServiceResponse<MethodInfoDTO>> GetMethodInfoAsync(SearchMethodRequest searchMethodRequest,
            CancellationToken cancellationToken)
        {
            try
            {
                MethodInfo methodInfo = await _dbManager?.MethodInfoRepository
                    ?.RetrieveAsync(methodInfo => methodInfo.MethodName == searchMethodRequest.MethodName,
                    new System.Linq.Expressions.Expression<Func<MethodInfo, object>>[]
                    {
                        methodInfo => methodInfo.SubMethodInfoSet.Where(sub => sub.SubMethodName == searchMethodRequest.SubMethodName),
                        methodInfo => methodInfo.MethodHeaders
                    });

                if (methodInfo == null)
                {
                    string errorMessage = $"Метод с именем \"{searchMethodRequest.MethodName}\" не найден.";
                    _logger.LogError(errorMessage);

                    return new ServiceResponse<MethodInfoDTO>
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        ErrorMessage = errorMessage
                    };
                }

                SubMethodInfo subMethodInfo = methodInfo.SubMethodInfoSet?.FirstOrDefault();

                if (subMethodInfo == null)
                {
                    string errorMessage = $"Метод с именем \"{searchMethodRequest.MethodName}\" не содержит информации о подметодах.";
                    _logger.LogError(errorMessage);

                    return new ServiceResponse<MethodInfoDTO>
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        ErrorMessage = errorMessage
                    };
                }

                return new ServiceResponse<MethodInfoDTO>
                {
                    IsSuccess = true,
                    Data = new MethodInfoDTO
                    {
                        MethodId = methodInfo.MethodId,
                        MethodName = methodInfo.MethodName,
                        SubMethodName = subMethodInfo.SubMethodName,
                        MethodPath = methodInfo.MethodPath,
                        SubMethodPath = subMethodInfo.SubMethodPath,
                        ApiBaseUri = methodInfo.ApiBaseUri,
                        ApiPrefix = methodInfo.ApiPrefix,
                        ApiTimeout = methodInfo.ApiTimeout,
                        MethodHeaders = methodInfo.MethodHeaders?.ToList()
                    }
                };
            }
            catch (Exception ex)
            {
                string headerMessage = "Во время поиска информации о запрошенном методе возникла непредвиденная ошибка.";

                _logger.LogError(ex, headerMessage);
                return new ServiceResponse<MethodInfoDTO>
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    ErrorMessage = ex.BuildExceptionMessage(headerMessage)
                };
            }
        }
    }
}
