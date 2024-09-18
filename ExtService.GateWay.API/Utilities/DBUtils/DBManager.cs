using ExtService.GateWay.API.Abstractions.Repositories;
using ExtService.GateWay.API.Abstractions.UnitsOfWork;
using ExtService.GateWay.DBContext;

namespace ExtService.GateWay.API.Utilities.DBUtils
{
    /// <summary>
    /// Class to manage repositories using unit of work pattern.
    /// DBManager is better to register as scoped service.
    /// </summary>
    public class DBManager : IDBManager
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IServiceProvider _scopedProvider;

        public DBManager(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;

            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                string message = "HttpContext can't be null.";

                throw new ArgumentNullException(message);
            }

            _scopedProvider = httpContext.RequestServices;
        }

        public IIdentificationRepository IdentificationRepository { get => _scopedProvider.GetRequiredService<IIdentificationRepository>(); }
        public IBillingConfigRepository BillingConfigRepository { get => _scopedProvider.GetRequiredService<IBillingConfigRepository>(); }
        public IBillingRepository BillingRepository { get => _scopedProvider.GetRequiredService<IBillingRepository>(); }
        public IMethodInfoRepository MethodInfoRepository { get => _scopedProvider.GetRequiredService<IMethodInfoRepository>(); }
        public INotificationInfoRepository NotificationInfoRepository { get => _scopedProvider.GetRequiredService<INotificationInfoRepository>(); }

        public async Task<int> CommitAsync()
        {
            return await _scopedProvider.GetRequiredService<GateWayContext>().SaveChangesAsync();
        }
    }
}
