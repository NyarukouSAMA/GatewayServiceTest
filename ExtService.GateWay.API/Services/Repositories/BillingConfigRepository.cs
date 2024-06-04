using ExtService.GateWay.API.Abstractions.Repositories;
using ExtService.GateWay.DBContext;
using ExtService.GateWay.DBContext.DBModels;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ExtService.GateWay.API.Services.Repositories
{
    public class BillingConfigRepository : IBillingConfigRepository
    {
        private readonly GateWayContext _gateWayContext;
        public BillingConfigRepository(GateWayContext gateWayContext)
        {
            _gateWayContext = gateWayContext;
        }

        public Task<BillingConfig> RetrieveAsync(Expression<Func<BillingConfig, bool>> criteria)
        {
            return _gateWayContext?.BillingConfigSet?.FirstOrDefaultAsync(criteria);
        }
    }
}
