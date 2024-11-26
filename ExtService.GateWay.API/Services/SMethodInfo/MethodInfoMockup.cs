using ExtService.GateWay.API.Abstractions.Services;
using ExtService.GateWay.API.Models.Common;
using ExtService.GateWay.API.Models.DTO;
using ExtService.GateWay.API.Models.ServiceModels;
using ExtService.GateWay.DBContext.DBModels;

namespace ExtService.GateWay.API.Services.SMethodInfo
{
    public class MethodInfoMockup : IMethodInfoService
    {
        public async Task<ServiceResponse<MethodInfoDTO>> GetMethodInfoAsync(SearchMethodRequest searchMethodRequest,
            CancellationToken cancellationToken)
        {
            return await Task.FromResult(new ServiceResponse<MethodInfoDTO>
            {
                Data = new MethodInfoDTO
                {
                    MethodId = Guid.NewGuid(),
                    MethodName = searchMethodRequest.MethodName,
                    SubMethodName = searchMethodRequest.SubMethodName,
                    MethodHeaders = new List<MethodHeaders>
                    {
                        new MethodHeaders
                        {
                            HeaderName = "Content-Type",
                            Plugin = new Plugins
                            {
                                PluginId = Guid.NewGuid(),
                                PluginName = "DefaultHeaderHandler",
                            },
                            PluginLinks = new List<PluginLinks>
                            {
                                new PluginLinks
                                {
                                    LinkId = Guid.NewGuid(),
                                    Parameter = new PluginParameters
                                    {
                                        ParameterId = Guid.NewGuid(),
                                        ParameterName = "Content-Type",
                                        ParameterValue = "application/json"
                                    }
                                }
                            }
                        }
                    },
                    ApiTimeout = 15,
                    ApiBaseUri = "https://api.example.com",
                    ApiPrefix = "api/v1",
                    MethodPath = "example/method",
                    SubMethodPath = "endpoint"
                },
                StatusCode = StatusCodes.Status200OK,
                IsSuccess = true
            });
        }
    }
}
