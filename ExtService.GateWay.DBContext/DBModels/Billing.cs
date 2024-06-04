namespace ExtService.GateWay.DBContext.DBModels
{
    public class Billing
    {
        public Guid BillingId { get; set; }
        public int RequestLimit { get; set; }
        public int RequestCount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        //M-to-1 links
        public Guid IdentificationId { get; set; }
        public Identification Identification { get; set; }
        public Guid MethodId { get; set; }
        public MethodInfo Method { get; set; }
        public Guid BillingConfigId { get; set; }
        public BillingConfig BillingConfig { get; set; }
    }
}
