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

        public async Task<MethodInfo> RetrieveAsync(Func<IQueryable<MethodInfo>, IQueryable<MethodInfo>> queryFunc)
        {
            IQueryable<MethodInfo> query = _gateWayContext?.MethodInfoSet;

            if (queryFunc != null)
            {
                query = queryFunc(query);
            }

            return await query?.FirstOrDefaultAsync();
        }
    }
}
