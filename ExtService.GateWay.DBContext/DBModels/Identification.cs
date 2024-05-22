namespace ExtService.GateWay.DBContext.DBModels
{
    public class Identification
    {
        public Guid IdentificationId { get; set; }
        public string ClientId { get; set; }
        public string EnvName { get; set; }
        //M-to-1 links
        public Guid SystemId { get; set; }
        public SystemInfo SystemInfo { get; set; }
        //1-to-M links
        public ICollection<Billing> BillingSet { get; set; }
    }
}
