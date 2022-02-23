using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context
{
    public class CostCategory
    {
        public CostCategory()
        {
            this.Cost = new HashSet<Cost>();
        }
        public int Id { get; set; }
        public string CostCategoryCode { get; set; }
        public string CostCategoryName { get; set; }
        public string Explanation { get; set; }


        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }

        [InverseProperty("CostCategory")]
        public virtual ICollection<Cost> Cost { get; set; }
    }
}
