using ExtService.GateWay.DBContext.DBModels;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace ExtService.GateWay.API.Abstractions.Repositories
{
    public interface INotificationInfoRepository
    {
        Task<NotificationInfo> RetrieveAsync(Expression<Func<NotificationInfo, bool>> criteria,
            Expression<Func<NotificationInfo, object>>[] includes = null);

        Task<int> UpdateAsync(Expression<Func<SetPropertyCalls<NotificationInfo>, SetPropertyCalls<NotificationInfo>>> entitySetter,
            Expression<Func<NotificationInfo, bool>> updateCriteria = null);
    }
}
