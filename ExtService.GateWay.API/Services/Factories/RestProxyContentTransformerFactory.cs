using ExtService.GateWay.API.Abstractions.Factories;
using ExtService.GateWay.API.Abstractions.Services;

namespace ExtService.GateWay.API.Services.Factories
{
    public class RestProxyContentTransformerFactory : IRestProxyContentTransformerFactory
    {
        private readonly IServiceProvider _serviceProvider;
        public RestProxyContentTransformerFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IProxyContentTransformer GetRestProxyContentTransformer(string httpMethod)
        {
            return _serviceProvider.GetRequiredKeyedService<IProxyContentTransformer>(httpMethod);
        }
    }
}
