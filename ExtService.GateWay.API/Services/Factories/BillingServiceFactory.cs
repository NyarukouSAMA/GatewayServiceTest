using ExtService.GateWay.API.Abstractions.Factories;
using ExtService.GateWay.API.Abstractions.Services;
using ExtService.GateWay.API.Models.Options;
using ExtService.GateWay.API.Services.SBilling;
using Microsoft.Extensions.Options;

namespace ExtService.GateWay.API.Services.Factories
{
    public class BillingServiceFactory : IBillingServiceFactory
    {
        private readonly MockupOptions _mockupOptions;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public BillingServiceFactory(
            IHttpContextAccessor httpContextAccessor,
            IOptions<MockupOptions> mockupOptions)
        {
            _mockupOptions = mockupOptions.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        public IBillingService GetBillingService()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                string message = "HttpContext can't be null.";

                throw new ArgumentNullException(message);
            }

            var scopedProvider = httpContext.RequestServices;

            if (_mockupOptions.BillingMockup)
            {
                return scopedProvider.GetRequiredService<BillingServiceMockup>();
            }
            else
            {
                return scopedProvider.GetRequiredService<CheckAndIncrementCounter>();
            }
        }
    }
}
