using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Production
{
    public class MachinePreProcessModel
    {
        public int Id { get; set; }
        public int? MachineId { get; set; }
        public int? PreProcessTypeId { get; set; }

        public int? LineNumber { get; set; }
        public bool? IsRequired { get; set; }
    }
}
