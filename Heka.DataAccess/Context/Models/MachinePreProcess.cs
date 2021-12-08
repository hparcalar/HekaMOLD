using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context
{
    public class MachinePreProcess
    {
        public int Id { get; set; }

        [ForeignKey("Machine")]
        public Nullable<int> MachineId { get; set; }

        [ForeignKey("PreProcessType")]
        public Nullable<int> PreProcessTypeId { get; set; }

        public Nullable<int> LineNumber { get; set; }
        public Nullable<bool> IsRequired { get; set; }

        public virtual Machine Machine { get; set; }
        public virtual PreProcessType PreProcessType { get; set; }
    }
}
