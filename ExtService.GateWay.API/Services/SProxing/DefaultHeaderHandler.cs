using ExtService.GateWay.API.Abstractions.Services;
using ExtService.GateWay.API.Models.Common;
using ExtService.GateWay.DBContext.DBModels;

namespace ExtService.GateWay.API.Services.SProxing
{
    public class DefaultHeaderHandler : IPlugin
    {
        public Task<ServiceResponse<string>> ExecuteAsync(
            MethodHeaders methodHeader,
            Dictionary<string, PluginParameters> pluginParameters,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(new ServiceResponse<string>
            {
                IsSuccess = true,
                StatusCode = StatusCodes.Status200OK,
                Data = pluginParameters[methodHeader.HeaderName]?.ParameterValue
            });
        }
    }
}
