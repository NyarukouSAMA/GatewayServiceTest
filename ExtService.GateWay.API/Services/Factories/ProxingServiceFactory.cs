using ExtService.GateWay.API.Abstractions.Factories;
using ExtService.GateWay.API.Abstractions.Services;
using ExtService.GateWay.API.Constants;
using ExtService.GateWay.API.Models.Options;
using ExtService.GateWay.API.Services.SProxing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ExtService.GateWay.API.Services.Factories
{
    public class ProxingServiceFactory : IProxingServiceFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly MockupOptions _mockupOptions;
        public ProxingServiceFactory(IServiceProvider serviceProvider, IOptions<MockupOptions> mockupOptions)
        {
            _mockupOptions = mockupOptions.Value;
            _serviceProvider = serviceProvider;
        }

        public IProxingService GetProxingService()
        {
            if (_mockupOptions.ProxyMockup)
            {
                return _serviceProvider.GetRequiredKeyedService<IProxingService>(ServiceNames.ProxingMockupName);
            }
            else
            {
                return _serviceProvider.GetRequiredKeyedService<IProxingService>(ServiceNames.ProxingServiceName);
            }
        }
    }
}
