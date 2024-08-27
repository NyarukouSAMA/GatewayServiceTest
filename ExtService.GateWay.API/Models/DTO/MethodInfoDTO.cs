using ExtService.GateWay.DBContext.DBModels;

namespace ExtService.GateWay.API.Models.DTO
{
    public class MethodInfoDTO
    {
        public Guid MethodId { get; set; }
        public string MethodName { get; set; }
        public string SubMethodName { get; set; }
        public string MethodPath { get; set; }
        public string SubMethodPath { get; set; }
        public string ApiBaseUri { get; set; }
        public string ApiPrefix { get; set; }
        public List<MethodHeaders> MethodHeaders { get; set; }
        public int ApiTimeout { get; set; }
    }
}
