using ExtService.GateWay.API.Abstractions.Factories;
using ExtService.GateWay.API.Abstractions.Services;
using ExtService.GateWay.API.Models.Options;
using ExtService.GateWay.API.Services.SLimitCheck;
using Microsoft.Extensions.Options;

namespace ExtService.GateWay.API.Services.Factories
{
    public class LimitCheckServiceFactory : ILimitCheckServiceFactory
    {
        private readonly MockupOptions _mockupOptions;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public LimitCheckServiceFactory(
            IHttpContextAccessor httpContextAccessor,
            IOptions<MockupOptions> mockupOptions)
        {
            _mockupOptions = mockupOptions.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        public ILimitCheckService GetLimitCheckService()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                string message = "HttpContext can't be null.";

                throw new ArgumentNullException(message);
            }

            var scopedProvider = httpContext.RequestServices;
            if (_mockupOptions.LimitCheckMockup)
            {
                return scopedProvider.GetRequiredService<LimitCheckMockup>();
            }
            else
            {
                return scopedProvider.GetRequiredService<LimitCheckService>();
            }
        }
    }
}
