using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context
{
    public partial class MachinePlanView
    {
        public int Id { get; set; }
        public Nullable<DateTime> ViewDate { get; set; }

        [ForeignKey("Plant")]
        public Nullable<int> PlantId { get; set; }

        public virtual Plant Plant { get; set; }
    }
}
