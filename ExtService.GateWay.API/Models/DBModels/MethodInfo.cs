namespace ExtService.GateWay.API.Models.DBModels
{
    public class MethodInfo
    {
        public Guid MethodId { get; set; }
        public string MethodName { get; set; }
        //1-to-M links
        public ICollection<Billing> BillingSet { get; set; }
    }
}
