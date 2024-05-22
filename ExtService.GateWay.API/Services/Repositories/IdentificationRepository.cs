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

        public Task<Identification> RetrieveAsync(Expression<Func<Identification, bool>> criteria)
        {
            return _gateWayContext?.IdentificationSet?.FirstOrDefaultAsync(criteria);
        }
    }
}
