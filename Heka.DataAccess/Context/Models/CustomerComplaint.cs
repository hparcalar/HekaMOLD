using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context
{
    public class CustomerComplaint
    {
        public int Id { get; set; }
        public string FormNo { get; set; }
        public DateTime? FormDate { get; set; }
        public int? IncomingType { get; set; }

        [ForeignKey("Firm")]
        public int? FirmId { get; set; }

        public string CustomerDocumentNo { get; set; }
        public string Explanation { get; set; }
        public string Notes { get; set; }
        public int? FormStatus { get; set; }

        public DateTime? ActionDate { get; set; }
        public DateTime? ClosedDate { get; set; }

        [ForeignKey("PreventiveAction")]
        public int? PreventiveActionId { get; set; }

        public virtual Firm Firm { get; set; }
        public virtual PreventiveAction PreventiveAction { get; set; }
    }
}
