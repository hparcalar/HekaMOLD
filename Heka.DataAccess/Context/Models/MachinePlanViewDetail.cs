using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context
{
    public partial class MachinePlanViewDetail
    {
        public int Id { get; set; }
        [ForeignKey("MachinePlanView")]
        public Nullable<int> MachinePlanViewId { get; set; }

        [ForeignKey("Machine")]
        public Nullable<int> MachineId { get; set; }

        [ForeignKey("WorkOrderDetail")]
        public Nullable<int> WorkOrderDetailId { get; set; }
        public Nullable<int> OrderNo { get; set; }

        public virtual MachinePlanView MachinePlanView { get; set; }
        public virtual Machine Machine { get; set; }
        public virtual WorkOrderDetail WorkOrderDetail { get; set; }
    }
}
