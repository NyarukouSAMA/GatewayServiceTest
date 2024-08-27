using ExtService.GateWay.DBContext.DBModels;
using System.Linq.Expressions;

namespace ExtService.GateWay.API.Abstractions.Repositories
{
    public interface IBillingRepository
    {
        Task<int> InsertAsync(Billing billing);
        Task<Billing> RetrieveAsync(Expression<Func<Billing, bool>> criteria);
        Task<IEnumerable<Billing>> RetrieveMultipleAsync(Expression<Func<Billing, bool>> criteria);
    }
}
