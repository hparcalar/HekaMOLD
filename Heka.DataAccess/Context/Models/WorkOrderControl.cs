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

    public partial class WorkOrderControl
    {
        public int Id { get; set; }

        [ForeignKey("WorkOrder")]
        public Nullable<int> WorkOrderId { get; set; }

        [ForeignKey("WorkOrderDetail")]
        public Nullable<int> WorkOrderDetailId { get; set; }

        [ForeignKey("WorkOrderControlType")]
        public Nullable<int> ControlTypeId { get; set; }
        public string ControlValue { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }
    
        public virtual WorkOrderControlType WorkOrderControlType { get; set; }
        public virtual WorkOrder WorkOrder { get; set; }
        public virtual WorkOrderDetail WorkOrderDetail { get; set; }
    }
}