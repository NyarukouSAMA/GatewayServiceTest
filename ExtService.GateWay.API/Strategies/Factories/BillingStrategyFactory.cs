using ExtService.GateWay.API.Abstractions.Factories;
using ExtService.GateWay.API.Abstractions.Strategy;
using ExtService.GateWay.API.Models.Options;
using ExtService.GateWay.API.Strategies.SBilling;
using Microsoft.Extensions.Options;

namespace ExtService.GateWay.API.Strategies.Factories
{
    public class BillingStrategyFactory : IBillingStrategyFactory
    {
        private readonly MockupOptions _mockupOptions;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public BillingStrategyFactory(
            IHttpContextAccessor httpContextAccessor,
            IOptions<MockupOptions> mockupOptions)
        {
            _mockupOptions = mockupOptions.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        public IBillingStrategy GetBillingStrategy()
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
