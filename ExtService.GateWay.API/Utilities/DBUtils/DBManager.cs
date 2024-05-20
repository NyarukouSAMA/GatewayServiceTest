using ExtService.GateWay.API.Abstractions.Repositories;
using ExtService.GateWay.API.Abstractions.UnitsOfWork;

namespace ExtService.GateWay.API.Utilities.DBUtils
{
    public class DBManager : IDBManager
    {
        private readonly GateWayContext _context;
        public DBManager(GateWayContext context,
            IBillingRepository billingRepository,
            IIdentificationRepository identificationRepository,
            IMethodInfoRepository methodInfoRepository)
        {
            _context = context;
            BillingRepository = billingRepository;
            IdentificationRepository = identificationRepository;
            MethodInfoRepository = methodInfoRepository;
        }

        public IIdentificationRepository IdentificationRepository { get; }
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
