using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Production
{
    public class ShiftTargetModel
    {
        public int Id { get; set; }
        public int? ShiftId { get; set; }
        public int? MachineId { get; set; }
        public DateTime? TargetDate { get; set; }
        public int? TargetCount { get; set; }

        #region VISUAL ELEMENTS
        public string ShiftCode { get; set; }
        public string ShiftName { get; set; }
        public string TargetDateStr { get; set; }
        public string MachineCode { get; set; }
        public string MachineName { get; set; }
        #endregion
    }
}
