using ExtService.GateWay.DBContext.DBModels;
using System.Linq.Expressions;

namespace ExtService.GateWay.API.Abstractions.Repositories
{
    public interface IMethodInfoRepository
    {
        public Task<MethodInfo> RetrieveAsync(Func<IQueryable<MethodInfo>, IQueryable<MethodInfo>> queryFunc);
    }
}
