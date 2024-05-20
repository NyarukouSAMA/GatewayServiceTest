using ExtService.GateWay.API.Abstractions.Repositories;
using ExtService.GateWay.API.Models.DBModels;
using ExtService.GateWay.API.Utilities.DBUtils;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ExtService.GateWay.API.Services.Repositories
{
    public class MethodInfoRepository : IMethodInfoRepository
    {
        private readonly GateWayContext _gateWayContext;

        public MethodInfoRepository(GateWayContext gateWayContext)
        {
            _gateWayContext = gateWayContext;
        }

        public async Task<MethodInfo> RetrieveAsync(Expression<Func<MethodInfo, bool>> criteria)
        {
            return await _gateWayContext?.MethodInfoSet?.FirstOrDefaultAsync(criteria);
        }
    }
}
