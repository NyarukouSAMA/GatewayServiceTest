using ExtService.GateWay.API.Models.Common;
using ExtService.GateWay.API.Models.HandlerModels;
using ExtService.GateWay.API.Models.ServiceModels;

namespace ExtService.GateWay.API.Abstractions.Services
{
    public interface ILimitCheckService
    {
        Task<ServiceResponse<LimitCheckResponse>> CheckLimitAsync(LimitNotificationHandlerModel limitCheckRequest, CancellationToken cancellationToken);
    }
}
