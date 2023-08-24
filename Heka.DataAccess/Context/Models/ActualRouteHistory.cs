namespace Heka.DataAccess.Context
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class ActualRouteHistory
    {
        public int Id { get; set; }

        [ForeignKey("WorkOrderDetail")]
        public Nullable<int> WorkOrderDetailId { get; set; }

        [ForeignKey("Process")]
        public Nullable<int> ProcessId { get; set; }

        [ForeignKey("ProcessGroup")]
        public Nullable<int> ProcessGroupId { get; set; }

        [ForeignKey("Machine")]
        public Nullable<int> MachineId { get; set; }

        [ForeignKey("MachineGroup")]
        public Nullable<int> MachineGroupId { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<int> ProcessStatus { get; set; }
        public Nullable<int> StartUserId { get; set; }
        public Nullable<int> EndUserId { get; set; }
        public string Explanation { get; set; }
    
        public virtual Process Process { get; set; }
        public virtual ProcessGroup ProcessGroup { get; set; }
        public virtual Machine Machine { get; set; }
        public virtual MachineGroup MachineGroup { get; set; }
        public virtual WorkOrderDetail WorkOrderDetail { get; set; }
    }
}
