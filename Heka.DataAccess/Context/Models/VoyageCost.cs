using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context
{
    public partial class VoyageCost
    {
        public VoyageCost()
        {
            this.VoyageCostDetail = new HashSet<VoyageCostDetail>();
        }
        public int Id { get; set; }
        [ForeignKey("Voyage")]
        public int VoyageId { get; set; }

        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }

        public virtual Voyage Voyage { get; set; }

        [InverseProperty("VoyageCost")]
        public virtual ICollection<VoyageCostDetail> VoyageCostDetail { get; set; }

    }
}
