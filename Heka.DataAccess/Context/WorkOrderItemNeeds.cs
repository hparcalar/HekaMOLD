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
    
    public partial class WorkOrderItemNeeds
    {
        public int Id { get; set; }
        public Nullable<int> WorkOrderDetailId { get; set; }
        public Nullable<int> WorkOrderId { get; set; }
        public Nullable<int> ItemId { get; set; }
        public Nullable<decimal> Quantity { get; set; }
        public Nullable<decimal> RemainingNeedsQuantity { get; set; }
        public Nullable<System.DateTime> CalculatedDate { get; set; }
    
        public virtual Item Item { get; set; }
        public virtual WorkOrder WorkOrder { get; set; }
        public virtual WorkOrderDetail WorkOrderDetail { get; set; }
    }
}
