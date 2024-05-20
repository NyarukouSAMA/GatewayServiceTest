using ExtService.GateWay.API.Abstractions.Repositories;
using ExtService.GateWay.API.Models.DBModels;
using ExtService.GateWay.API.Utilities.DBUtils;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ExtService.GateWay.API.Services.Repositories
{
    public class IdentificationRepository : IIdentificationRepository
    {
        GateWayContext _gateWayContext;
        public IdentificationRepository(GateWayContext gateWayContext)
        {
            _gateWayContext = gateWayContext;
        }

        public async Task<Identification> RetrieveAsync(Expression<Func<Identification, bool>> criteria)
        {
            return await _gateWayContext?.IdentificationSet?.FirstOrDefaultAsync(criteria);
        }
    }
}
