using ExtService.GateWay.API.Abstractions.Repositories;
using ExtService.GateWay.DBContext;
using ExtService.GateWay.DBContext.DBModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace ExtService.GateWay.API.Services.Repositories
{
    public class NotificationInfoRepository : INotificationInfoRepository
    {
        private readonly GateWayContext _gateWayContext;

        public NotificationInfoRepository(GateWayContext gateWayContext)
        {
            _gateWayContext = gateWayContext;
        }

        public async Task<NotificationInfo> RetrieveAsync(Expression<Func<NotificationInfo, bool>> criteria, Expression<Func<NotificationInfo, object>>[] includes = null)
        {
            IQueryable<NotificationInfo> query = _gateWayContext?.NotificationInfoSet;

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query?.Include(include);
                }
            }

            return await query?.FirstOrDefaultAsync(criteria);
        }

        public async Task<int> UpdateAsync(Expression<Func<SetPropertyCalls<NotificationInfo>, SetPropertyCalls<NotificationInfo>>> entitySetter,
            Expression<Func<NotificationInfo, bool>> updateCriteria)
        {
            IQueryable<NotificationInfo> query = _gateWayContext?.NotificationInfoSet;

            if (updateCriteria != null)
            {
                query = query?.Where(updateCriteria);
            }

            return await query?.ExecuteUpdateAsync(entitySetter);
        }
    }
}
