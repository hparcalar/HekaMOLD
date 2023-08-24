using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Production
{
    public class MachinePreProcessHistoryModel
    {
        public int Id { get; set; }
        public int? MachineId { get; set; }
        public int? MoldId { get; set; }
        public int? CreatedUserId { get; set; }
        public int? PreProcessTypeId { get; set; }
        public int? WorkOrderDetailId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Duration { get; set; }
    }
}
