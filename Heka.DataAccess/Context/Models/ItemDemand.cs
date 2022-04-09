using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context
{
    public class ItemDemand
    {
        public int Id { get; set; }

        [ForeignKey("Item")]
        public Nullable<int> ItemId { get; set; }

        public Nullable<decimal> DemandQuantity { get; set; }
        public Nullable<decimal> SuppliedQuantity { get; set; }

        [ForeignKey("WorkOrderDetail")]
        public Nullable<int> WorkOrderDetailId { get; set; }
        public string Explanation { get; set; }

        public Nullable<DateTime> DemandDate { get; set; }
        public Nullable<DateTime> SupplyDate { get; set; }

        [ForeignKey("DemandedUser")]
        public Nullable<int> DemandedUserId { get; set; }

        [ForeignKey("SupplierUser")]
        public Nullable<int> SupplierUserId { get; set; }

        public Nullable<int> DemandStatus { get; set; }

        public virtual Item Item { get; set; }
        public virtual WorkOrderDetail WorkOrderDetail { get; set; }
        public virtual User DemandedUser { get; set; }
        public virtual User SupplierUser { get; set; }
    }
}
