using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Maintenance
{
    public class MachineMaintenanceInstructionModel
    {
        public int Id { get; set; }
        public int? MachineId { get; set; }
        public string UnitName { get; set; }
        public string PeriodType { get; set; }
        public string ToDoList { get; set; }
        public string Responsible { get; set; }
        public int? LineNumber { get; set; }

        #region VISUAL ELEMENTS
        public string MachineCode { get; set; }
        public string MachineName { get; set; }
        #endregion
    }
}
