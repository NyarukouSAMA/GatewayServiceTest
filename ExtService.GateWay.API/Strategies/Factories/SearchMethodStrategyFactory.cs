using ExtService.GateWay.API.Abstractions.Factories;
using ExtService.GateWay.API.Abstractions.Strategy;
using ExtService.GateWay.API.Models.Options;
using ExtService.GateWay.API.Strategies.SMethodInfo;
using Microsoft.Extensions.Options;

namespace ExtService.GateWay.API.Strategies.Factories
{
    public class SearchMethodStrategyFactory : ISearchMethodStrategyFactory
    {
        private readonly MockupOptions _mockupOptions;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SearchMethodStrategyFactory(IHttpContextAccessor httpContextAccessor,
            IOptions<MockupOptions> mockupOptions)
        {
            _mockupOptions = mockupOptions.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        public IMethodInfoStrategy GetMethodInfoStrategy()
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext == null)
            {
                string message = "HttpContext can't be null.";

                throw new ArgumentNullException(message);
            }

            var scopedProvider = httpContext.RequestServices;

            if (_mockupOptions.MethodInfoMockup)
            {
                return scopedProvider.GetRequiredService<MethodInfoMockup>();
            }
            else
            {
                return scopedProvider.GetRequiredService<GetMethodByName>();
            }
        }

    }
}
