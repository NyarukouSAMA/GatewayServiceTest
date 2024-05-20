using ExtService.GateWay.API.Abstractions.Strategy;
using ExtService.GateWay.API.Models.Common;
using ExtService.GateWay.API.Models.DBModels;
using ExtService.GateWay.API.Models.ServiceRequests;

namespace ExtService.GateWay.API.Strategies.SMethodInfo
{
    public class MethodInfoMockup : IMethodInfoStrategy
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
