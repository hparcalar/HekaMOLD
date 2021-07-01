using HekaMOLD.Business.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Production
{
    public class MachineModel : IDataObject
    {
        public int Id { get; set; }
        public string MachineCode { get; set; }
        public string MachineName { get; set; }
        public int? MachineType { get; set; }
        public int? PlantId { get; set; }
        public bool? IsActive { get; set; }

        public MachinePlanModel[] Plans { get; set; }

        #region VISUAL ELEMENTS
        public string CreatedDateStr { get; set; }
        #endregion
    }
}
