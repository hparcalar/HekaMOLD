using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Crm
{
    public class CustomerComplaintModel
    {
        public int Id { get; set; }
        public string FormNo { get; set; }
        public DateTime? FormDate { get; set; }
        public int? IncomingType { get; set; }
        public int? FirmId { get; set; }

        public string CustomerDocumentNo { get; set; }
        public string Explanation { get; set; }
        public string Notes { get; set; }
        public int? FormStatus { get; set; }

        public DateTime? ActionDate { get; set; }
        public DateTime? ClosedDate { get; set; }
        public int? PreventiveActionId { get; set; }

        #region VISUAL ELEMENTS
        public string FormDateText { get; set; }
        public string ActionDateText { get; set; }
        public string ClosedDateText { get; set; }
        public string FormStatusText { get; set; }
        public string PreventiveActionFormNo { get; set; }
        public string IncomingTypeText { get; set; }
        #endregion
    }
}
