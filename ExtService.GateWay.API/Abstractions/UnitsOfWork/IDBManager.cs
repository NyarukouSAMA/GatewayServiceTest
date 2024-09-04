using ExtService.GateWay.API.Abstractions.Repositories;

namespace ExtService.GateWay.API.Abstractions.UnitsOfWork
{
    public interface IDBManager
    {
        IIdentificationRepository IdentificationRepository { get; }
        IBillingConfigRepository BillingConfigRepository { get; }
        IBillingRepository BillingRepository { get; }
        IMethodInfoRepository MethodInfoRepository { get; }
        INotificationInfoRepository NotificationInfoRepository { get; }
        Task<int> CommitAsync();
    }
}
