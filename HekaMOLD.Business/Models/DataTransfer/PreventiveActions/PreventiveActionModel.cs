using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.PreventiveActions
{
    public class PreventiveActionModel
    {
        public int Id { get; set; }
        public string ApplicantName { get; set; }
        public string ApplicantIdentity { get; set; }
        public string ApplicantFirmName { get; set; }
        public string ApplicantTitle { get; set; }

        public string ActionType { get; set; }

        public DateTime? FormDate { get; set; }
        public string FormNo { get; set; }
        public string Declaration { get; set; }
        public string RootCause { get; set; }
        public string SolutionProposal { get; set; }
        public int? FormResult { get; set; }
        public int? ApproverUserId { get; set; }
        public int? ClosingUserId { get; set; }

        #region VISUAL ELEMENTS
        public PreventiveActionDetailModel[] Details { get; set; }
        public string ApproverUserName { get; set; }
        public string ClosingUserName { get; set; }
        public string FormResultText { get; set; }
        public string FormDateText { get; set; }
        public string ApproveState { get; set; }
        public string CloseState { get; set; }
        #endregion
    }
}
