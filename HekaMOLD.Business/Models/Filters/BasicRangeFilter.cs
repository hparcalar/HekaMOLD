using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.Filters
{
    public class BasicRangeFilter
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Search { get; set; }
        public int MachineId { get; set; }
    }
}
