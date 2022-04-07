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

    public partial class Incident
    {
        public int Id { get; set; }

        [ForeignKey("Machine")]
        public Nullable<int> MachineId { get; set; }

        [ForeignKey("IncidentCategory")]
        public Nullable<int> IncidentCategoryId { get; set; }
        public Nullable<int> IncidentStatus { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }

        [ForeignKey("CreatedUser")]
        public Nullable<int> CreatedUserId { get; set; }

        [ForeignKey("StartedUser")]
        public Nullable<int> StartedUserId { get; set; }

        [ForeignKey("EndUser")]
        public Nullable<int> EndUserId { get; set; }

        [ForeignKey("Shift")]
        public Nullable<int> ShiftId { get; set; }
        public Nullable<System.DateTime> ShiftBelongsToDate { get; set; }

        public virtual User CreatedUser { get; set; }
        public virtual User StartedUser { get; set; }
        public virtual User EndUser { get; set; }
        public virtual IncidentCategory IncidentCategory { get; set; }
        public virtual Shift Shift { get; set; }
        public virtual Machine Machine { get; set; }
    }
}
