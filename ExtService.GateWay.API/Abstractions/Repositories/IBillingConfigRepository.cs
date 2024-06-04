using ExtService.GateWay.DBContext.DBModels;
using System.Linq.Expressions;

namespace ExtService.GateWay.API.Abstractions.Repositories
{
    public interface IBillingConfigRepository
    {
        Task<BillingConfig> RetrieveAsync(Expression<Func<BillingConfig, bool>> criteria);
    }
}
