using ExtService.GateWay.DBContext.DBModels;
using System.Linq.Expressions;

namespace ExtService.GateWay.API.Abstractions.Repositories
{
    public interface IMethodInfoRepository
    {
        Task<MethodInfo> RetrieveAsync(Expression<Func<MethodInfo, bool>> criteria);
    }
}
