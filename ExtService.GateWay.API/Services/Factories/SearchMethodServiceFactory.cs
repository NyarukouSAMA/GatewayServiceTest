using ExtService.GateWay.API.Abstractions.Factories;
using ExtService.GateWay.API.Abstractions.Services;
using ExtService.GateWay.API.Models.Options;
using ExtService.GateWay.API.Services.SMethodInfo;
using Microsoft.Extensions.Options;

namespace ExtService.GateWay.API.Services.Factories
{
    public class SearchMethodServiceFactory : ISearchMethodServiceFactory
    {
        private readonly MockupOptions _mockupOptions;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SearchMethodServiceFactory(IHttpContextAccessor httpContextAccessor,
            IOptions<MockupOptions> mockupOptions)
        {
            _mockupOptions = mockupOptions.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        public IMethodInfoService GetMethodInfoService()
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
