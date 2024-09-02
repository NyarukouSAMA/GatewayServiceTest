using ExtService.GateWay.API.Abstractions.Services;
using ExtService.GateWay.API.Models.Common;
using ExtService.GateWay.API.Models.HandlerModels;
using ExtService.GateWay.API.Models.ServiceModels;

namespace ExtService.GateWay.API.Services.SLimitCheck
{
    public class LimitCheckMockup : ILimitCheckService
    {
        public Task<ServiceResponse<LimitCheckResponse>> CheckLimitAsync(LimitNotificationHandlerModel limitCheckRequest, CancellationToken cancellationToken)
        {
            return Task.FromResult(new ServiceResponse<LimitCheckResponse>
            {
                IsSuccess = true,
                StatusCode = 200,
                Data = new LimitCheckResponse
                {
                    SendNotification = false
                }
            });
        }
    }
}
