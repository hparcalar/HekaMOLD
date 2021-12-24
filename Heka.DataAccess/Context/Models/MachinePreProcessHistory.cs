using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context
{
    public partial class MachinePreProcessHistory
    {
        public int Id { get; set; }

        [ForeignKey("Machine")]
        public Nullable<int> MachineId { get; set; }

        [ForeignKey("Mold")]
        public Nullable<int> MoldId { get; set; }

        [ForeignKey("User")]
        public Nullable<int> CreatedUserId { get; set; }

        [ForeignKey("PreProcessType")]
        public Nullable<int> PreProcessTypeId { get; set; }

        [ForeignKey("WorkOrderDetail")]
        public Nullable<int> WorkOrderDetailId { get; set; }
        public Nullable<DateTime> StartDate { get; set; }
        public Nullable<DateTime> EndDate { get; set; }
        public Nullable<int> Duration { get; set; }

        public virtual Machine Machine { get; set; }
        public virtual Mold Mold { get; set; }
        public virtual User User { get; set; }
        public virtual PreProcessType PreProcessType { get; set; }
        public virtual WorkOrderDetail WorkOrderDetail { get; set; }
    }
}
