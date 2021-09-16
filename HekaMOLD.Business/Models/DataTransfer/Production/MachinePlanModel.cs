using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Production
{
    public class MachinePlanModel
    {
        public int Id { get; set; }
        public int? MachineId { get; set; }
        public int? WorkOrderDetailId { get; set; }
        public int? OrderNo { get; set; }
        public WorkOrderDetailModel WorkOrder { get; set; }
        public WorkOrderSerialModel[] Serials { get; set; }
    }
}
