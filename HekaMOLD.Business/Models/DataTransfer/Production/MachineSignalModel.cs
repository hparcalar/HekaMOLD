using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Production
{
    public class MachineSignalModel
    {
        public int Id { get; set; }
        public int? MachineId { get; set; }
        public int? WorkOrderDetailId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? ShiftId { get; set; }
        public int? Duration { get; set; }
        public int? SignalStatus { get; set; }
        public DateTime? ShiftBelongsToDate { get; set; }
    }
}
