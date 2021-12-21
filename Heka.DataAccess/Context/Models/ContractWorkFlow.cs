using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context
{
    public class ContractWorkFlow
    {
        public int Id { get; set; }

        [ForeignKey("WorkOrderDetail")]
        public Nullable<int> WorkOrderDetailId { get; set; }

        [ForeignKey("DeliveredReceiptDetail")]
        public Nullable<int> DeliveredDetailId { get; set; }

        [ForeignKey("ReceivedReceiptDetail")]
        public Nullable<int> ReceivedDetailId { get; set; }
        public Nullable<DateTime> FlowDate { get; set; }

        public virtual WorkOrderDetail WorkOrderDetail { get; set; }
        public virtual ItemReceiptDetail DeliveredReceiptDetail { get; set; }
        public virtual ItemReceiptDetail ReceivedReceiptDetail { get; set; }
    }
}
