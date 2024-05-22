using ExtService.GateWay.API.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using ExtService.GateWay.DBContext;
using ExtService.GateWay.DBContext.DBModels;

namespace ExtService.GateWay.API.Services.Repositories
{
    public class BillingRepository : IBillingRepository
    {
        private readonly GateWayContext _gateWayContext;
        public BillingRepository(GateWayContext gateWayContext)
        {
            _gateWayContext = gateWayContext;
        }

        public Task<Billing> RetrieveAsync(Expression<Func<Billing, bool>> criteria)
        {
            return _gateWayContext?.BillingSet?.FirstOrDefaultAsync(criteria);
        }

        public async Task<IEnumerable<Billing>> RetrieveMultipleAsync(Expression<Func<Billing, bool>> criteria)
        {
            return await _gateWayContext?.BillingSet?.Where(criteria)?.ToListAsync() ?? new List<Billing>();
        }
    }
}
