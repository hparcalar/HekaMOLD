using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Production
{
    public class RouteItemModel
    {
        public int Id { get; set; }
        public int? RouteId { get; set; }
        public int? ProcessId { get; set; }
        public int? ProcessGroupId { get; set; }
        public int? LineNumber { get; set; }
        public string Explanation { get; set; }
        public int? MachineId { get; set; }
        public int? MachineGroupId { get; set; }
    }
}
