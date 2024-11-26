using ExtService.GateWay.API.Abstractions.Services;

namespace ExtService.GateWay.API.Abstractions.Factories
{
    public interface IPluginFactory
    {
        IPlugin GetPlugin(string pluginName);
    }
}
