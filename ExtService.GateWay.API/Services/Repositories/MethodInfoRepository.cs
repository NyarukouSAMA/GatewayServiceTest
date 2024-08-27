using ExtService.GateWay.API.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using ExtService.GateWay.DBContext;
using ExtService.GateWay.DBContext.DBModels;

namespace ExtService.GateWay.API.Services.Repositories
{
    public class MethodInfoRepository : IMethodInfoRepository
    {
        private readonly GateWayContext _gateWayContext;

        public MethodInfoRepository(GateWayContext gateWayContext)
        {
            _gateWayContext = gateWayContext;
        }

        public async Task<MethodInfo> RetrieveAsync(Expression<Func<MethodInfo, bool>> criteria,
            Expression<Func<MethodInfo, object>>[] includes = null)
        {
            IQueryable<MethodInfo> query = _gateWayContext?.MethodInfoSet;

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query?.Include(include);
                }
            }

            return await query?.FirstOrDefaultAsync(criteria);
        }
    }
}
