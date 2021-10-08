using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Core
{
    public class SystemParameterModel
    {
        public int Id { get; set; }
        public string PrmCode { get; set; }
        public string PrmValue { get; set; }
        public int? PlantId { get; set; }
    }
}
