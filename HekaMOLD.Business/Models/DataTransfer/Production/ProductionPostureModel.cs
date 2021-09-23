using HekaMOLD.Business.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Production
{
    public class ProductionPostureModel : IDataObject
    {
        public int Id { get; set; }
        public int? MachineId { get; set; }
        public int? WorkOrderDetailId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? PostureStatus { get; set; }
        public int? PostureCategoryId { get; set; }
        public string Reason { get; set; }
        public string Explanation { get; set; }

        #region VISUAL ELEMENTS
        public string PostureStatusStr { get; set; }
        public string PostureCategoryCode { get; set; }
        public string PostureCategoryName { get; set; }
        public string MachineCode { get; set; }
        public string MachineName { get; set; }
        public string CreatedDateStr { get; set; }
        public string StartDateStr { get; set; }
        public string EndDateStr { get; set; }
        public string CreatedUserName { get; set; }
        #endregion
    }
}
