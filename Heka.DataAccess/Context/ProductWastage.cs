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
    
    public partial class ProductWastage
    {
        public int Id { get; set; }
        public Nullable<int> WorkOrderDetailId { get; set; }
        public Nullable<int> ProductId { get; set; }
        public Nullable<int> MachineId { get; set; }
        public Nullable<System.DateTime> EntryDate { get; set; }
        public Nullable<decimal> Quantity { get; set; }
        public Nullable<int> WastageStatus { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }
        public Nullable<int> ShiftId { get; set; }
        public Nullable<System.DateTime> ShiftBelongsToDate { get; set; }
    
        public virtual Item Item { get; set; }
        public virtual Shift Shift { get; set; }
        public virtual Machine Machine { get; set; }
        public virtual WorkOrderDetail WorkOrderDetail { get; set; }
    }
}
