using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.PreventiveActions
{
    public class PreventiveActionDetailModel
    {
        public int Id { get; set; }
        public int? PreventiveActionId { get; set; }
        public int? ResponsibleUserId { get; set; }

        public string ActionText { get; set; }
        public string Notes { get; set; }
        public DateTime? DeadlineDate { get; set; }
        public int? ActionStatus { get; set; }

        #region VISUAL ELEMENTS
        public string ResponsibleUserName { get; set; }
        public string ActionStatusText { get; set; }
        public string DeadlineDateText { get; set; }
        public bool NewDetail { get; set; } = false;
        #endregion
    }
}
