using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.Virtual
{
    public class TrialPlanModel
    {
        public int WorkOrderId { get; set; }
        public int WorkOrderDetailId { get; set; }
        public int MachineId { get; set; }
        public decimal? Quantity { get; set; }
        public string TrialFirmExplanation { get; set; }
        public string TrialProductExplanation { get; set; }
    }
}
