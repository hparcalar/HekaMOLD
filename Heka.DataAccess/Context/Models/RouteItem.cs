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

    public partial class RouteItem
    {
        public int Id { get; set; }

        [ForeignKey("Route")]
        public Nullable<int> RouteId { get; set; }

        [ForeignKey("Process")]
        public Nullable<int> ProcessId { get; set; }

        [ForeignKey("ProcessGroup")]
        public Nullable<int> ProcessGroupId { get; set; }
        public Nullable<int> LineNumber { get; set; }
        public string Explanation { get; set; }

        [ForeignKey("Machine")]
        public Nullable<int> MachineId { get; set; }

        [ForeignKey("MachineGroup")]
        public Nullable<int> MachineGroupId { get; set; }
    
        public virtual Process Process { get; set; }
        public virtual ProcessGroup ProcessGroup { get; set; }
        public virtual Route Route { get; set; }
        public virtual Machine Machine { get; set; }
        public virtual MachineGroup MachineGroup { get; set; }
    }
}