using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context
{
    public partial class PreProcessType
    {
        public int Id { get; set; }
        public string PreProcessCode { get; set; }
        public string PreProcessName { get; set; }
        public Nullable<bool> HasMoldSelection { get; set; }
        public Nullable<bool> TwoStepProcess { get; set; }

        [ForeignKey("Plant")]
        public Nullable<int> PlantId { get; set; }

        public virtual Plant Plant { get; set; }
    }
}
