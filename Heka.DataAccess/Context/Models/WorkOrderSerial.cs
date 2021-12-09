namespace Heka.DataAccess.Context
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class WorkOrderSerial
    {
        public int Id { get; set; }

        [ForeignKey("WorkOrderDetail")]
        public Nullable<int> WorkOrderDetailId { get; set; }

        [ForeignKey("WorkOrder")]
        public Nullable<int> WorkOrderId { get; set; }
        public string SerialNo { get; set; }
        public Nullable<int> SerialType { get; set; }
        public Nullable<bool> IsGeneratedBySignal { get; set; }
        public Nullable<int> SerialStatus { get; set; }
        public Nullable<decimal> FirstQuantity { get; set; }
        public Nullable<decimal> LiveQuantity { get; set; }
        public Nullable<int> InPackageQuantity { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }
        public Nullable<System.DateTime> QualityChangedDate { get; set; }

        [ForeignKey("QualityUser")]
        public Nullable<int> QualityUserId { get; set; }

        [ForeignKey("Shift")]
        public Nullable<int> ShiftId { get; set; }

        [ForeignKey("ItemReceiptDetail")]
        public Nullable<int> ItemReceiptDetailId { get; set; }
        public Nullable<int> QualityStatus { get; set; }
        public string QualityExplanation { get; set; }

        [ForeignKey("Warehouse")]
        public Nullable<int> TargetWarehouseId { get; set; }
    
        public virtual ItemReceiptDetail ItemReceiptDetail { get; set; }
        public virtual Shift Shift { get; set; }
        public virtual Warehouse Warehouse { get; set; }
        public virtual WorkOrder WorkOrder { get; set; }
        public virtual WorkOrderDetail WorkOrderDetail { get; set; }
        public virtual User QualityUser { get; set; }
    }
}
