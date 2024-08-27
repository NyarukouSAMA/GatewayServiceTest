using ExtService.GateWay.DBContext.DBModels;

namespace ExtService.GateWay.API.Models.HandlerModels
{
    public class IdentificationHandlerResponse
    {
        public Guid IdentificationId { get; set; }
        public Guid MethodId { get; set; }
        public string SystemName { get; set; }
        public string RequestUri { get; set; }
        public List<MethodHeaders> MethodHeaders { get; set; }
        public int ApiTimeout { get; set; }
    }
}
