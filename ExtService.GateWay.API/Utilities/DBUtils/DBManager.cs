using ExtService.GateWay.API.Abstractions.Repositories;
using ExtService.GateWay.API.Abstractions.UnitsOfWork;
using ExtService.GateWay.API.Services.Repositories;
using ExtService.GateWay.DBContext;

namespace ExtService.GateWay.API.Utilities.DBUtils
{
    public class DBManager : IDBManager
    {
        private readonly GateWayContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly Lazy<IIdentificationRepository> _identificationRepository;
        private readonly Lazy<IBillingConfigRepository> _billingConfigRepository;
        private readonly Lazy<IBillingRepository> _billingRepository;
        private readonly Lazy<IMethodInfoRepository> _methodInfoRepository;
        private readonly Lazy<INotificationInfoRepository> _notificationInfoRepository;

        public DBManager(GateWayContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;

            _identificationRepository = new Lazy<IIdentificationRepository>(Initialize<IIdentificationRepository>((scopedProvider) =>
            {
                return scopedProvider.GetRequiredService<IdentificationRepository>();
            }));

            _billingConfigRepository = new Lazy<IBillingConfigRepository>(Initialize<IBillingConfigRepository>((scopedProvider) =>
            {
                return scopedProvider.GetRequiredService<BillingConfigRepository>();
            }));

            _billingRepository = new Lazy<IBillingRepository>(Initialize<IBillingRepository>((scopedProvider) =>
            {
                return scopedProvider.GetRequiredService<BillingRepository>();
            }));

            _methodInfoRepository = new Lazy<IMethodInfoRepository>(Initialize<IMethodInfoRepository>((scopedProvider) =>
            {
                return scopedProvider.GetRequiredService<MethodInfoRepository>();
            }));

            _notificationInfoRepository = new Lazy<INotificationInfoRepository>(Initialize<INotificationInfoRepository>((scopedProvider) =>
            {
                return scopedProvider.GetRequiredService<NotificationInfoRepository>();
            }));
        }

        public IIdentificationRepository IdentificationRepository { get => _identificationRepository.Value; }
        public IBillingConfigRepository BillingConfigRepository { get => _billingConfigRepository.Value; }
        public IBillingRepository BillingRepository { get => _billingRepository.Value; }
        public IMethodInfoRepository MethodInfoRepository { get => _methodInfoRepository.Value; }
        public INotificationInfoRepository NotificationInfoRepository { get => _notificationInfoRepository.Value; }

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }

        /// <summary>
        /// Initialize repository with repository initializer using scoped by httpContext provider.
        /// </summary>
        /// <typeparam name="TRepoInterface">Repository interface</typeparam>
        /// <param name="repositoryInitializer">Function to initialize repository</param>
        /// <returns>Instanciated repository converted to interface</returns>
        /// <exception cref="ArgumentNullException">Can throw ArgumentNullException if HttpContext is null</exception>
        private TRepoInterface Initialize<TRepoInterface>(Func<IServiceProvider, TRepoInterface> repositoryInitializer = null)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                string message = "HttpContext can't be null.";

                throw new ArgumentNullException(message);
            }

            var scopedProvider = httpContext.RequestServices;

            return repositoryInitializer(scopedProvider);
        }
    }
}
