using ExtService.GateWay.API.Abstractions.Factories;
using ExtService.GateWay.API.Abstractions.Strategy;
using ExtService.GateWay.API.Models.Options;
using ExtService.GateWay.API.Strategies.SClientIdentification;
using Microsoft.Extensions.Options;

namespace ExtService.GateWay.API.Strategies.Factories
{
    public class ClientIdentificationStrategyFactory : IClientIdentificationStrategyFactory
    {
        private readonly MockupOptions _mockupOptions;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ClientIdentificationStrategyFactory(
            IHttpContextAccessor httpContextAccessor,
            IOptions<MockupOptions> mockupOptions)
        {
            _mockupOptions = mockupOptions.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        public IClientIdentificationStrategy GetClientIdentificationStrategy()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                string message = "HttpContext can't be null.";

                throw new ArgumentNullException(message);
            }

            var scopedProvider = httpContext.RequestServices;

            if (_mockupOptions.ClientIdentificationMockup)
            {
                return scopedProvider.GetRequiredService<ClientIdentificationMockup>();
            }
            else
            {
                try
                {
                    return scopedProvider.GetRequiredService<CheckUserByClientId>();
                }
                catch (Exception ex)
                {
                    throw new Exception("ClientIdentificationStrategyFactory: Error while getting CheckUserByClientId strategy", ex);
                }
            }
        }
    }
}
