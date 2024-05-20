using ExtService.GateWay.API.Abstractions.Repositories;
using ExtService.GateWay.API.Models.DBModels;
using ExtService.GateWay.API.Utilities.DBUtils;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ExtService.GateWay.API.Services.Repositories
{
    public class BillingRepository : IBillingRepository
    {
        private readonly GateWayContext _gateWayContext;
        public BillingRepository(GateWayContext gateWayContext)
        {
            _gateWayContext = gateWayContext;
        }

        public async Task<Billing> RetrieveAsync(Expression<Func<Billing, bool>> criteria)
        {
            return await _gateWayContext?.BillingSet?.FirstOrDefaultAsync(criteria);
        }

        public async Task<IEnumerable<Billing>> RetrieveMultipleAsync(Expression<Func<Billing, bool>> criteria)
        {
            return await _gateWayContext?.BillingSet?.Where(criteria)?.ToListAsync() ?? new List<Billing>();
        }
    }
}
