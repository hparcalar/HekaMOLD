using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Production
{
    public class PreProcessTypeModel
    {
        public int Id { get; set; }
        public string PreProcessCode { get; set; }
        public string PreProcessName { get; set; }
        public bool? HasMoldSelection { get; set; }
        public bool? TwoStepProcess { get; set; }
    }
}
