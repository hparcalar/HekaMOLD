using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Maintenance
{
    public class MaintenancePlanModel
    {
        public int Id { get; set; }
        public Nullable<int> MachineId { get; set; }
        public Nullable<DateTime> StartDate { get; set; }
        public Nullable<DateTime> EndDate { get; set; }
        public string Subject { get; set; }
        public string Explanation { get; set; }
        public string ResultExplanation { get; set; }
        public Nullable<int> ResponsibleId { get; set; }
        public int PlanStatus { get; set; }

        #region VISUAL ELEMENTS
        public string MachineCode { get; set; }
        public string MachineName { get; set; }
        public string UserCode { get; set; }
        public string UserName { get; set; }
        public string PlanStatusStr { get; set; }
        public string StartDateStr { get; set; }
        public string EndDateStr { get; set; }
        #endregion
    }
}
