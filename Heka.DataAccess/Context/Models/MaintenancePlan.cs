using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context
{
    public partial class MaintenancePlan
    {
        public int Id { get; set; }

        [ForeignKey("Machine")]
        public Nullable<int> MachineId { get; set; }
        public Nullable<DateTime> StartDate { get; set; }
        public Nullable<DateTime> EndDate { get; set; }
        public string Subject { get; set; }
        public string Explanation { get; set; }
        public string ResultExplanation { get; set; }

        [ForeignKey("User")]
        public Nullable<int> ResponsibleId { get; set; }
        public int PlanStatus { get; set; }

        public virtual Machine Machine { get; set; }
        public virtual User User { get; set; }
    }
}
