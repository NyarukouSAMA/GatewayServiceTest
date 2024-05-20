using ExtService.GateWay.API.Abstractions.Factories;
using ExtService.GateWay.API.Abstractions.Strategy;
using ExtService.GateWay.API.Models.Options;
using ExtService.GateWay.API.Strategies.SBilling;
using Microsoft.Extensions.Options;

namespace ExtService.GateWay.API.Strategies.Factories
{
    public class BillingStrategyFactory : IBillingStrategyFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly MockupOptions _mockupOptions;
        public BillingStrategyFactory(IServiceProvider serviceProvider, IOptions<MockupOptions> mockupOptions)
        {
            _mockupOptions = mockupOptions.Value;
            _serviceProvider = serviceProvider;
        }

        public IBillingStrategy GetBillingStrategy()
        {
            if (_mockupOptions.BillingMockup)
            {
                return _serviceProvider.GetRequiredService<BillingServiceMockup>();
            }
            else
            {
                return _serviceProvider.GetRequiredService<CheckAndIncrementCounter>();
            }
        }
    }
}
