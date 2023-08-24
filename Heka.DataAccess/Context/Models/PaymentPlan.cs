using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heka.DataAccess.Context
{
    public class PaymentPlan
    {
        public int Id { get; set; }
        public string PaymentPlanCode { get; set; }
        public string PaymentPlanName { get; set; }
        public string IntegrationCode { get; set; }
    }
}
