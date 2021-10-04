using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.MachineService.Models
{
    public class MachineStatus
    {
        public int MachineId { get; set; }
        public DateTime? LastCycleEnd { get; set; }
        public int? PostureExpirationCycleCount { get; set; }
        public bool PostureRequestSent { get; set; }
    }
}
