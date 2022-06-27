namespace Heka.DataAccess.Context
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class MachineSignal
    {
        public int Id { get; set; }

        [ForeignKey("Machine")]
        public Nullable<int> MachineId { get; set; }

        [ForeignKey("WorkOrderDetail")]
        public Nullable<int> WorkOrderDetailId { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<int> Duration { get; set; }
        public Nullable<int> SignalStatus { get; set; }

        [ForeignKey("User")]
        public Nullable<int> OperatorId { get; set; }

        [ForeignKey("Shift")]
        public Nullable<int> ShiftId { get; set; }
        public Nullable<System.DateTime> ShiftBelongsToDate { get; set; }
    
        public virtual Shift Shift { get; set; }
        public virtual Machine Machine { get; set; }
        public virtual WorkOrderDetail WorkOrderDetail { get; set; }
        public virtual User User { get; set; }
    }
}
