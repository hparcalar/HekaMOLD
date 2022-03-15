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
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class DeliveryPlan
    {
        public int Id { get; set; }

        [ForeignKey("WorkOrderDetail")]
        public Nullable<int> WorkOrderDetailId { get; set; }

        [ForeignKey("ItemOrderDetail")]
        public Nullable<int> ItemOrderDetailId { get; set; }
        public Nullable<System.DateTime> PlanDate { get; set; }
        public Nullable<int> OrderNo { get; set; }
        public Nullable<int> PlanStatus { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }
        public Nullable<decimal> Quantity { get; set; }

        public virtual WorkOrderDetail WorkOrderDetail { get; set; }
        public virtual ItemOrderDetail ItemOrderDetail { get; set; }
    }
}
