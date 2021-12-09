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

    public partial class ProductionPosture
    {
        public int Id { get; set; }

        [ForeignKey("Machine")]
        public Nullable<int> MachineId { get; set; }

        [ForeignKey("WorkOrderDetail")]
        public Nullable<int> WorkOrderDetailId { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<int> PostureStatus { get; set; }
        public string Reason { get; set; }
        public string Explanation { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }

        [ForeignKey("PostureCategory")]
        public Nullable<int> PostureCategoryId { get; set; }

        [ForeignKey("Shift")]
        public Nullable<int> ShiftId { get; set; }
        public Nullable<System.DateTime> ShiftBelongsToDate { get; set; }
    
        public virtual Shift Shift { get; set; }
        public virtual Machine Machine { get; set; }
        public virtual PostureCategory PostureCategory { get; set; }
        public virtual WorkOrderDetail WorkOrderDetail { get; set; }
    }
}