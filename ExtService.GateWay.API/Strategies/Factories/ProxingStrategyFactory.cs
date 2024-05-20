using ExtService.GateWay.API.Abstractions.Factories;
using ExtService.GateWay.API.Abstractions.Strategy;
using ExtService.GateWay.API.Models.Options;
using ExtService.GateWay.API.Strategies.SProxing;
using Microsoft.Extensions.Options;

namespace ExtService.GateWay.API.Strategies.Factories
{
    public class ProxingStrategyFactory : IProxingStrategyFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly MockupOptions _mockupOptions;
        public ProxingStrategyFactory(IServiceProvider serviceProvider, IOptions<MockupOptions> mockupOptions)
        {
            _mockupOptions = mockupOptions.Value;
            _serviceProvider = serviceProvider;
        }

        public IProxingStrategy GetProxingStrategy()
        {
            if (_mockupOptions.ProxyMockup)
            {
                return _serviceProvider.GetRequiredService<ProxyMockup>();
            }
            else
            {
                return _serviceProvider.GetRequiredService<ServiceProxing>();
            }
        }
    }
}
