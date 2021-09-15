using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Maintenance
{
    public class MachineMaintenanceInstructionEntryModel
    {
        public int Id { get; set; }
        public int? InstructionId { get; set; }
        public int? CreatedUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool? IsChecked { get; set; }
        public string Explanation { get; set; }

        #region VISUAL ELEMENTS
        public string MachineCode { get; set; }
        public string MachineName { get; set; }
        public string ToDoList { get; set; }
        public string CreatedUserName { get; set; }
        public string CreatedDateStr { get; set; }
        #endregion
    }
}
