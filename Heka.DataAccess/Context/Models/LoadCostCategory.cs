using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context
{
    public partial class LoadCostCategory
    {
        public LoadCostCategory()
        {
            this.ItemLoadCost = new HashSet<ItemLoadCost>();
        }
        public int Id { get; set; }
        public string LoadCostCategoryCode { get; set; }
        public string LoadCostCategoryName { get; set; }

        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }

        [InverseProperty("LoadCostCategory")]
        public virtual ICollection<ItemLoadCost> ItemLoadCost { get; set; }
    }
}
