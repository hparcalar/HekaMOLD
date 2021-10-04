using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.EmployeeStats
{
    public class UserWorkOrderHistoryModel
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? WorkOrderDetailId { get; set; }
        public int? MachineId { get; set; }
        public decimal? StartQuantity { get; set; }
        public decimal? EndQuantity { get; set; }
        public decimal? FinishedQuantity { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        #region VISUAL ELEMENTS
        public string StartDateStr { get; set; }
        public string EndDateStr { get; set; }
        public string MachineCode { get; set; }
        public string MachineName { get; set; }
        public string WorkOrderNo { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string UserCode { get; set; }
        public string UserName { get; set; }
        #endregion
    }
}
