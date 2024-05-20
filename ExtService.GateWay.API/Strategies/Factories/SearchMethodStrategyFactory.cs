using ExtService.GateWay.API.Abstractions.Factories;
using ExtService.GateWay.API.Abstractions.Strategy;
using ExtService.GateWay.API.Models.Options;
using ExtService.GateWay.API.Strategies.SMethodInfo;
using Microsoft.Extensions.Options;

namespace ExtService.GateWay.API.Strategies.Factories
{
    public class SearchMethodStrategyFactory : ISearchMethodStrategyFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly MockupOptions _mockupOptions;

        public SearchMethodStrategyFactory(IServiceProvider serviceProvider,
            IOptions<MockupOptions> mockupOptions)
        {
            _serviceProvider = serviceProvider;
            _mockupOptions = mockupOptions.Value;
        }

        public IMethodInfoStrategy GetMethodInfoStrategy()
        {
            if (_mockupOptions.MethodInfoMockup)
            {
                return _serviceProvider.GetRequiredService<MethodInfoMockup>();
            }
            else
            {
                return _serviceProvider.GetRequiredService<GetMethodByName>();
            }
        }

    }
}
