using System;

namespace Heka.DataAccess.Context.Models
{
    public partial class RoundCostCategory
    {
        public RoundCostCategory()
        {

        }
        public int Id { get; set; }
        public string RoundCostCategoryCode { get; set; }
        public string RoundCostCategoryName { get; set; }

        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }
    }
}
