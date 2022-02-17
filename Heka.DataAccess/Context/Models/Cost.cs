using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context.Models
{
    public class Cost
    {
        public Cost()
        {

        }
        public int? Id { get; set; }
        public string CostCode { get; set; }
        public string CostName { get; set; }
        public int? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? OverallTotal { get; set; }
        public string Explanation { get; set; }

        [ForeignKey("ForexType")]
        public int? ForexTypeId { get; set; }

        [ForeignKey("UnitType")]
        public int? UnitTypeId { get; set; }

        [ForeignKey("CostCategory")]
        public int? CostCategoryId { get; set; }

        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }

        public virtual ForexType ForexType { get; set; }
        public virtual UnitType UnitType { get; set; }
        public virtual CostCategory CostCategory { get; set; }


    }
}
