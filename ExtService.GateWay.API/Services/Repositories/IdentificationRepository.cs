using ExtService.GateWay.API.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using ExtService.GateWay.DBContext;
using ExtService.GateWay.DBContext.DBModels;

namespace ExtService.GateWay.API.Services.Repositories
{
    public class IdentificationRepository : IIdentificationRepository
    {
        GateWayContext _gateWayContext;
        public IdentificationRepository(GateWayContext gateWayContext)
        {
            _gateWayContext = gateWayContext;
        }

        public Task<Identification> RetrieveAsync(Expression<Func<Identification, bool>> criteria,
            Expression<Func<Identification, object>>[] includes = null)
        {
            IQueryable<Identification> query = _gateWayContext?.IdentificationSet;

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query?.Include(include);
                }
            }

            return query?.FirstOrDefaultAsync(criteria);
        }
    }
}
