using ExtService.GateWay.API.Abstractions.Factories;
using ExtService.GateWay.API.Abstractions.Services;

namespace ExtService.GateWay.API.Services.Factories
{
    public class PluginFactory : IPluginFactory
    {
        private readonly IServiceProvider _serviceProvider;
        public PluginFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IPlugin GetPlugin(string pluginName)
        {
            return _serviceProvider.GetRequiredKeyedService<IPlugin>(pluginName);
        }
    }
}
