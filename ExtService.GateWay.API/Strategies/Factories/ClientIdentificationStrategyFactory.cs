using ExtService.GateWay.API.Abstractions.Factories;
using ExtService.GateWay.API.Abstractions.Strategy;
using ExtService.GateWay.API.Models.Options;
using ExtService.GateWay.API.Strategies.SClientIdentification;
using Microsoft.Extensions.Options;

namespace ExtService.GateWay.API.Strategies.Factories
{
    public class ClientIdentificationStrategyFactory : IClientIdentificationStrategyFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly MockupOptions _mockupOptions;
        public ClientIdentificationStrategyFactory(IServiceProvider serviceProvider, IOptions<MockupOptions> mockupOptions)
        {
            _mockupOptions = mockupOptions.Value;
            _serviceProvider = serviceProvider;
        }

        public IClientIdentificationStrategy GetClientIdentificationStrategy()
        {
            if (_mockupOptions.ClientIdentificationMockup)
            {
                return _serviceProvider.GetRequiredService<ClientIdentificationMockup>();
            }
            else
            {
                try
                {
                    return _serviceProvider.GetRequiredService<CheckUserByClientId>();
                }
                catch (Exception ex)
                {
                    throw new Exception("ClientIdentificationStrategyFactory: Error while getting CheckUserByClientId strategy", ex);
                }
            }
        }
    }
}
