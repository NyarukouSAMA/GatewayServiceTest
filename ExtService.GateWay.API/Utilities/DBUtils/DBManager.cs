using ExtService.GateWay.API.Abstractions.Repositories;
using ExtService.GateWay.API.Abstractions.UnitsOfWork;
using ExtService.GateWay.DBContext;

namespace ExtService.GateWay.API.Utilities.DBUtils
{
    public class DBManager : IDBManager
    {
        private readonly GateWayContext _context;
        public DBManager(GateWayContext context,
            IBillingConfigRepository billingConfigRepository,
            IBillingRepository billingRepository,
            IIdentificationRepository identificationRepository,
            IMethodInfoRepository methodInfoRepository)
        {
            _context = context;
            BillingConfigRepository = billingConfigRepository;
            BillingRepository = billingRepository;
            IdentificationRepository = identificationRepository;
            MethodInfoRepository = methodInfoRepository;
        }

        public IIdentificationRepository IdentificationRepository { get; }
        public IBillingConfigRepository BillingConfigRepository { get; }
        public IBillingRepository BillingRepository { get; }
        public IMethodInfoRepository MethodInfoRepository { get; }

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
