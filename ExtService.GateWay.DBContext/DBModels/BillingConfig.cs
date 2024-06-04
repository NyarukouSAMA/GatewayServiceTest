using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtService.GateWay.DBContext.DBModels
{
    public class BillingConfig
    {
        public Guid BillingConfigId { get; set; }
        public int PeriodInDays { get; set; }
        public int RequestLimitPerPeriod { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        //M-to-1 links
        public Guid IdentificationId { get; set; }
        public Identification Identification { get; set; }
        public Guid MethodId { get; set; }
        public MethodInfo Method { get; set; }
        //1-to-M links
        public ICollection<NotificationInfo> NotificationInfoSet { get; set; }
        public ICollection<Billing> BillingRecords { get; set; }
    }
}
