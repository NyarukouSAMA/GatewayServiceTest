using ExtService.GateWay.API.Models.Common;
using ExtService.GateWay.DBContext.DBModels;

namespace ExtService.GateWay.API.Abstractions.Services
{
    public interface IPlugin
    {
        Task<ServiceResponse<string>> ExecuteAsync(
            MethodHeaders methodHeader,
            Dictionary<string, PluginParameters> pluginParameters,
            CancellationToken cancellationToken);
    }
}
