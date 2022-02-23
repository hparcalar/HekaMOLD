using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context
{
    public class ItemVariant
    {
        public ItemVariant()
        {
            this.ItemKnitDensity = new HashSet<ItemKnitDensity>();
            this.KnitYarn = new HashSet<KnitYarn>();
        }

        public int Id { get; set; }
        public string ItemNo { get; set; }
        public string ItemName { get; set; }
        public Nullable<int> ItemType { get; set; }

        [ForeignKey("ItemCategory")]
        public Nullable<int> ItemCategoryId { get; set; }

        [ForeignKey("ItemGroup")]
        public Nullable<int> ItemGroupId { get; set; }

        [ForeignKey("Firm")]
        public Nullable<int> SupplierFirmId { get; set; }

        [ForeignKey("Plant")]
        public Nullable<int> PlantId { get; set; }


        public Nullable<int> TaxRate { get; set; }
        //Desen
        public int? Pattern { get; set; }
        //Ham
        public decimal? CrudeWidth { get; set; }
        public decimal? CrudeGramaj { get; set; }
        //Mamul En
        public decimal? ProductWidth { get; set; }
        public decimal? ProductGramaj { get; set; }
        //Cozgu Tel sayisi
        public decimal? WarpWireCount { get; set; }
        public decimal? MeterGramaj { get; set; }
        //Kesme	
        public Nullable<int> ItemCutType { get; set; }

        //Boyahane
        public Nullable<int> ItemDyeHouseType { get; set; }
        //konfeksiyon
        public Nullable<int> ItemApparelType { get; set; }
        //Kursun
        public Nullable<int> ItemBulletType { get; set; }
        public string AttemptNo { get; set; }
        public decimal? CombWidth { get; set; }
        //Atki Rapor Boyu
        public decimal? WeftReportLength { get; set; }
        //Cozgu Rapor Boyu
        public decimal? WarpReportLength { get; set; }
        //Ortalama Atki Sikligi
        public int? AverageWeftDensity { get; set; }
        //Ortalama Cozgu Sikligi
        public int? AverageWarpDensity { get; set; }

        [ForeignKey("WeavingDraft")]
        public int? WeavingDraftId { get; set; }

        [ForeignKey("ItemQualityType")]
        public int? ItemQualityTypeId { get; set; }

        [ForeignKey("Item")]
        public int? ItemId { get; set; }

        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }

        public virtual Firm Firm { get; set; }
        public virtual ItemCategory ItemCategory { get; set; }
        public virtual ItemGroup ItemGroup { get; set; }
        public virtual Plant Plant { get; set; }
        public virtual ItemQualityType ItemQualityType { get; set; }
        public virtual WeavingDraft WeavingDraft { get; set; }
        public virtual Item Item { get; set; }


        [InverseProperty("ItemVariant")]
        public virtual ICollection<ItemUnit> ItemUnit { get; set; }

        [InverseProperty("ItemVariant")]
        public virtual ICollection<ItemKnitDensity> ItemKnitDensity { get; set; }

        [InverseProperty("ItemVariant")]
        public virtual ICollection<KnitYarn> KnitYarn { get; set; }

    }
}
