namespace ExtService.GateWay.API.Models.ServiceModels
{
    public class BillingResponse
    {
        public Guid BillingId { get; set; }
        public int RequestLimit { get; set; }
        public int RequestCount { get; set; }
        public Guid BillingConfigId { get; set; }
    }
}
