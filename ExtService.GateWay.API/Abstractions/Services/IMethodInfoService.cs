using ExtService.GateWay.API.Models.Common;
using ExtService.GateWay.API.Models.DTO;
using ExtService.GateWay.API.Models.ServiceRequests;
using ExtService.GateWay.DBContext.DBModels;

namespace ExtService.GateWay.API.Abstractions.Services
{
    public interface IMethodInfoService
    {
        Task<ServiceResponse<MethodInfoDTO>> GetMethodInfoAsync(SearchMethodRequest searchMethodRequest, CancellationToken cancellationToken);
    }
}
