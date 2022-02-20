using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context
{
    public partial class ItemLoadCost
    {
        public ItemLoadCost()
        {

        }
        public int Id { get; set; }
        [ForeignKey("ItemLoad")]
        public int ItemLoadId { get; set; }
        [ForeignKey("LoadCostCategory")]
        public int LoadCostCategoryId { get; set; }
        public decimal? CostPrice { get; set; }
        [ForeignKey("ForexType")]
        public int? ForexTypeId { get; set; }
        public string Explanation { get; set; }

        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }

        public virtual ItemLoad ItemLoad { get; set; }
        public virtual LoadCostCategory LoadCostCategory { get; set; }
        public virtual ForexType ForexType { get; set; }

    }
}
