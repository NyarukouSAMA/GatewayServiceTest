using ExtService.GateWay.API.Abstractions.Services;
using ExtService.GateWay.API.Abstractions.UnitsOfWork;
using ExtService.GateWay.API.Helpers;
using ExtService.GateWay.API.Models.Common;
using ExtService.GateWay.API.Models.DTO;
using ExtService.GateWay.API.Models.ServiceModels;
using ExtService.GateWay.DBContext.DBFunctions.PGSQLAssignableFunctions;
using ExtService.GateWay.DBContext.DBModels;
using Microsoft.EntityFrameworkCore;

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
                MethodInfo methodInfo = await _dbManager?.MethodInfoRepository?.RetrieveAsync(query =>
                    query
                        .Where(MethodInfo => MethodInfo.MethodName == searchMethodRequest.MethodName)
                        .Include(MethodInfo => MethodInfo.SubMethodInfoSet
                            .Where(SubMethodInfo => SubMethodInfo.SubMethodName == searchMethodRequest.SubMethodName
                                && SubMethodInfo.HttpMethodName == searchMethodRequest.HttpMethod.Method))
                        .Include(MethodInfo => MethodInfo.MethodHeaders)
                            .ThenInclude(MethodHeaders => MethodHeaders.Plugin)
                        .Include(MethodInfo => MethodInfo.MethodHeaders)
                            .ThenInclude(MethodHeaders => MethodHeaders.PluginLinks)
                            .ThenInclude(pl => pl.Parameter)
                        .Select(MethodInfo => new MethodInfo
                        {
                            MethodId = MethodInfo.MethodId,
                            MethodName = MethodInfo.MethodName,
                            SubMethodInfoSet = MethodInfo.SubMethodInfoSet,
                            ApiBaseUri = MethodInfo.ApiBaseUri,
                            ApiPrefix = MethodInfo.ApiPrefix,
                            ApiTimeout = MethodInfo.ApiTimeout,
                            MethodPath = MethodInfo.MethodPath,
                            MethodHeaders = MethodInfo.MethodHeaders.Select(MethodHeaders => new MethodHeaders
                            {
                                HeaderName = MethodHeaders.HeaderName,
                                Plugin = MethodHeaders.Plugin,
                                PluginLinks = MethodHeaders.PluginLinks.Select(pl => new PluginLinks
                                {
                                    LinkId = pl.LinkId,
                                    Parameter = new PluginParameters
                                    {
                                        ParameterId = pl.Parameter.ParameterId,
                                        ParameterName = pl.Parameter.ParameterName,
                                        ParameterValue = DecryptPluginParameterValueFunction.Execute(pl.Parameter.ParameterValue)
                                    }
                                }).ToList()
                            }).ToList()
                        }));

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
