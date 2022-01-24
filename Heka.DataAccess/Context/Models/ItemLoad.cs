using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context.Models
{
    public partial class ItemLoad
    {
        public ItemLoad()
        {
            this.ItemLoadDetail = new HashSet<ItemLoadDetail>();
        }
        public int Id { get; set; }
        public string LoadCode { get; set; }
        public Nullable<System.DateTime> LoadDate { get; set; }
        public Nullable<System.DateTime> DischargeDate { get; set; }
        public int? OrderLoadStatusType { get; set; }
        public decimal? OveralWeight { get; set; }
        public decimal? OveralVolume { get; set; }
        public decimal? OveralLadametre { get; set; }
        public Nullable<decimal> OverallTotal { get; set; }
        public decimal? CalculationTypePrice { get; set; }

        public string Explanation { get; set; }

        [ForeignKey("FirmShipper")]
        public int? ShipperFirmId { get; set; }

        [ForeignKey("FirmBuyer")]
        public int? BuyerFirmId { get; set; }

        [ForeignKey("ItemOrder")]
        public Nullable<int> ItemOrderId { get; set; }

        [ForeignKey("Plant")]
        public Nullable<int> PlantId { get; set; }
        [ForeignKey("User")]
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }

        public virtual Plant Plant { get; set; }
        public virtual User User { get; set; }
        public virtual ItemOrder ItemOrder { get; set; }
        public virtual Firm FirmShipper { get; set; }
        public virtual Firm FirmBuyer { get; set; }

        [InverseProperty("ItemLoad")]
        public virtual ICollection<ItemLoadDetail> ItemLoadDetail { get; set; }

    }
}
