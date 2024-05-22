using ExtService.GateWay.API.Abstractions.Services;
using ExtService.GateWay.API.Models.Common;
using ExtService.GateWay.API.Models.ServiceRequests;
using ExtService.GateWay.DBContext.DBModels;

namespace ExtService.GateWay.API.Services.SMethodInfo
{
    public class MethodInfoMockup : IMethodInfoService
    {
        public async Task<ServiceResponse<MethodInfo>> GetMethodInfoAsync(SearchMethodRequest searchMethodRequest,
            CancellationToken cancellationToken)
        {
            return await Task.FromResult(new ServiceResponse<MethodInfo>
            {
                Data = new MethodInfo
                {
                    MethodId = Guid.NewGuid(),
                    MethodName = searchMethodRequest.MethodName
                },
                StatusCode = StatusCodes.Status200OK,
                IsSuccess = true
            });
        }
    }
}
