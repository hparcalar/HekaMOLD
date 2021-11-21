using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Production
{
    public class ActualRouteHistoryModel
    {
        public int Id { get; set; }
        public int? WorkOrderDetailId { get; set; }
        public int? RouteId { get; set; }
        public int? ProcessId { get; set; }
        public int? ProcessGroupId { get; set; }
        public int? MachineId { get; set; }
        public int? MachineGroupId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? ProcessStatus { get; set; }
        public int? StartUserId { get; set; }
        public int? EndUserId { get; set; }
        public string Explanation { get; set; }
    }
}
