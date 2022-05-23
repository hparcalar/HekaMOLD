using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context
{
    public class PreventiveAction
    {
        public PreventiveAction()
        {
            this.PreventiveActionDetail = new HashSet<PreventiveActionDetail>();
        }
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

        [ForeignKey("ApproverUser")]
        public int? ApproverUserId { get; set; }

        [ForeignKey("ClosingUser")]
        public int? ClosingUserId { get; set; }

        public virtual User ApproverUser { get; set; }
        public virtual User ClosingUser { get; set; }

        [InverseProperty("PreventiveAction")]
        public virtual ICollection<PreventiveActionDetail> PreventiveActionDetail { get; set; }
    }
}
