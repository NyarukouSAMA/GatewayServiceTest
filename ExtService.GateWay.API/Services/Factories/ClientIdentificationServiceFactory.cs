using ExtService.GateWay.API.Abstractions.Factories;
using ExtService.GateWay.API.Abstractions.Services;
using ExtService.GateWay.API.Models.Options;
using ExtService.GateWay.API.Services.SClientIdentification;
using Microsoft.Extensions.Options;

namespace ExtService.GateWay.API.Services.Factories
{
    public class ClientIdentificationServiceFactory : IClientIdentificationServiceFactory
    {
        private readonly MockupOptions _mockupOptions;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ClientIdentificationServiceFactory(
            IHttpContextAccessor httpContextAccessor,
            IOptions<MockupOptions> mockupOptions)
        {
            _mockupOptions = mockupOptions.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        public IClientIdentificationService GetClientIdentificationService()
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
