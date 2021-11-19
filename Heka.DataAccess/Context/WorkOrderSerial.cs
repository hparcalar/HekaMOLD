//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Heka.DataAccess.Context
{
    using System;
    using System.Collections.Generic;
    
    public partial class WorkOrderSerial
    {
        public int Id { get; set; }
        public Nullable<int> WorkOrderDetailId { get; set; }
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
        public Nullable<int> ShiftId { get; set; }
        public Nullable<int> ItemReceiptDetailId { get; set; }
        public Nullable<int> QualityStatus { get; set; }
        public string QualityExplanation { get; set; }
    
        public virtual ItemReceiptDetail ItemReceiptDetail { get; set; }
        public virtual WorkOrder WorkOrder { get; set; }
        public virtual WorkOrderDetail WorkOrderDetail { get; set; }
        public virtual Shift Shift { get; set; }
    }
}
