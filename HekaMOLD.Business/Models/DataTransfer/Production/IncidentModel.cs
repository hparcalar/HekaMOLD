using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Production
{
    public class IncidentModel
    {
        public int Id { get; set; }
        public int? MachineId { get; set; }
        public int? IncidentCategoryId { get; set; }
        public int? IncidentStatus { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? CreatedUserId { get; set; }
        public int? StartedUserId { get; set; }
        public int? EndUserId { get; set; }

        #region VISUAL ELEMENTS
        public string MachineCode { get; set; }
        public string MachineName { get; set; }
        public string IncidentStatusStr { get; set; }
        public string IncidentCategoryCode { get; set; }
        public string IncidentCategoryName { get; set; }
        public string CreatedOnlyDateStr { get; set; }
        public string CreatedDateStr { get; set; }
        public string StartDateStr { get; set; }
        public string EndDateStr { get; set; }
        public string CreatedUserName { get; set; }
        public string StartedUserName { get; set; }
        public string EndUserName { get; set; }
        #endregion
    }
}
