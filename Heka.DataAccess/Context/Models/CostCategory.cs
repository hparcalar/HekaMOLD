using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context
{
    public partial class CostCategory
    {
        public CostCategory()
        {
            this.VoyageCostDetail = new HashSet<VoyageCostDetail>();
            this.DriverAccountDetail = new HashSet<DriverAccountDetail>();
        }
        public int Id { get; set; }
        public string CostCategoryCode { get; set; }
        public string CostCategoryName { get; set; }

        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }

        [InverseProperty("CostCategory")]
        public virtual ICollection<VoyageCostDetail> VoyageCostDetail { get; set; }

        [InverseProperty("CostCategory")]
        public virtual ICollection<DriverAccountDetail> DriverAccountDetail { get; set; }
    }
}
