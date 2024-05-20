using ExtService.GateWay.API.Models.Common;
using ExtService.GateWay.API.Models.DBModels;
using MediatR;

namespace ExtService.GateWay.API.Models.ServiceRequests
{
    public class SearchMethodRequest : IRequest<ServiceResponse<MethodInfo>>
    {
        public string MethodName { get; set; } = string.Empty;
    }
}
