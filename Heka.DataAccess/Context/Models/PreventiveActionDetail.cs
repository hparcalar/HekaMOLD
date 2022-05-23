using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context
{
    public class PreventiveActionDetail
    {
        public int Id { get; set; }

        [ForeignKey("PreventiveAction")]
        public int? PreventiveActionId { get; set; }

        [ForeignKey("ResponsibleUser")]
        public int? ResponsibleUserId { get; set; }

        public string ActionText { get; set; }
        public string Notes { get; set; }
        public DateTime? DeadlineDate { get; set; }
        public int? ActionStatus { get; set; }

        public virtual PreventiveAction PreventiveAction { get; set; }
        public virtual User ResponsibleUser { get; set; }
    }
}
