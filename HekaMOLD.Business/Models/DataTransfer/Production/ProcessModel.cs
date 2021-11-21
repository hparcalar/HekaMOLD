using HekaMOLD.Business.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Production
{
    public class ProcessModel : IDataObject
    {
        public int Id { get; set; }
        public string ProcessCode { get; set; }
        public string ProcessName { get; set; }
        public int? PlantId { get; set; }
        public bool? IsActive { get; set; }
        public decimal? TheoreticalDuration { get; set; }
        public int? ProcessGroupId { get; set; }
    }
}
