using ExtService.GateWay.DBContext.DBModels;
using System.Linq.Expressions;

namespace ExtService.GateWay.API.Abstractions.Repositories
{
    public interface IIdentificationRepository
    {
        Task<Identification> RetrieveAsync(Expression<Func<Identification, bool>> criteria);
    }
}
