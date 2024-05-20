using ExtService.GateWay.API.Abstractions.Repositories;

namespace ExtService.GateWay.API.Abstractions.UnitsOfWork
{
    public interface IDBManager : IDisposable
    {
        IIdentificationRepository IdentificationRepository { get; }
        IBillingRepository BillingRepository { get; }
        IMethodInfoRepository MethodInfoRepository { get; }
        Task<int> CommitAsync();
    }
}
