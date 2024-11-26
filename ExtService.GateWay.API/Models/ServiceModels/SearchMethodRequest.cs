using ExtService.GateWay.API.Models.Common;
using ExtService.GateWay.DBContext.DBModels;
using MediatR;

namespace ExtService.GateWay.API.Models.ServiceModels
{
    public class SearchMethodRequest : IRequest<ServiceResponse<MethodInfo>>
    {
        public string MethodName { get; set; } = string.Empty;
        public string SubMethodName { get; set; } = string.Empty;
        public HttpMethod HttpMethod { get; set; }
    }
}
