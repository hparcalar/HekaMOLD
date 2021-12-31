namespace Heka.DataAccess.Context
{
    using Heka.DataAccess.Context.Models;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class Item
    {
        
        public Item()
        {
            this.EntryQualityData = new HashSet<EntryQualityData>();
            this.ItemLiveStatus = new HashSet<ItemLiveStatus>();
            this.ItemOrderDetail = new HashSet<ItemOrderDetail>();
            this.ItemOrderItemNeeds = new HashSet<ItemOrderItemNeeds>();
            this.ItemReceiptDetail = new HashSet<ItemReceiptDetail>();
            this.ItemRequestDetail = new HashSet<ItemRequestDetail>();
            this.ItemUnit = new HashSet<ItemUnit>();
            this.ItemWarehouse = new HashSet<ItemWarehouse>();
            
            this.MoldProduct = new HashSet<MoldProduct>();
            this.ProductQualityData = new HashSet<ProductQualityData>();
            this.ProductRecipe = new HashSet<ProductRecipe>();
            this.ProductRecipeDetail = new HashSet<ProductRecipeDetail>();
            this.WorkOrderItemNeeds = new HashSet<WorkOrderItemNeeds>();
            this.ItemSerial = new HashSet<ItemSerial>();
            this.WorkOrderDetail = new HashSet<WorkOrderDetail>();
            this.ProductWastage = new HashSet<ProductWastage>();
            this.FabricRecipe = new HashSet<FabricRecipe>();
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

        [ForeignKey("Mold")]
        public Nullable<int> MoldId { get; set; }

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
        public int? TestNo { get; set; }
        public decimal? CombWidth { get; set; }
        //Atki Rapor Boyu
        public decimal? WeftReportLength { get; set; }
        //Cozgu Rapor Boyu
        public decimal? WarpReportLength { get; set; }
        //Atki Sikligi
        public int? WeftDensity { get; set; }

        [ForeignKey("Machine")]
        public int? MachineId { get; set; }

        [ForeignKey("ItemQualityType")]
        public int? ItemQualityTypeId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }

        public virtual Firm Firm { get; set; }
        public virtual ItemCategory ItemCategory { get; set; }
        public virtual ItemGroup ItemGroup { get; set; }
        public virtual Mold Mold { get; set; }
        public virtual Plant Plant { get; set; }
        public virtual ItemQualityType ItemQualityType { get; set; }
        public virtual Machine Machine { get; set; }

        [InverseProperty("Item")]
        public virtual ICollection<ItemLiveStatus> ItemLiveStatus { get; set; }

        [InverseProperty("Item")]
        public virtual ICollection<ItemOrderDetail> ItemOrderDetail { get; set; }

        [InverseProperty("Item")]
        public virtual ICollection<ItemOrderItemNeeds> ItemOrderItemNeeds { get; set; }

        [InverseProperty("Item")]
        public virtual ICollection<ItemReceiptDetail> ItemReceiptDetail { get; set; }

        [InverseProperty("Item")]
        public virtual ICollection<ItemRequestDetail> ItemRequestDetail { get; set; }

        [InverseProperty("Item")]
        public virtual ICollection<ItemUnit> ItemUnit { get; set; }

        [InverseProperty("Item")]
        public virtual ICollection<ItemWarehouse> ItemWarehouse { get; set; }


        [InverseProperty("Item")]
        public virtual ICollection<MoldProduct> MoldProduct { get; set; }

        [InverseProperty("Item")]
        public virtual ICollection<ProductQualityData> ProductQualityData { get; set; }

        [InverseProperty("Item")]
        public virtual ICollection<ProductRecipe> ProductRecipe { get; set; }

        [InverseProperty("Item")]
        public virtual ICollection<ProductRecipeDetail> ProductRecipeDetail { get; set; }

        [InverseProperty("Item")]
        public virtual ICollection<WorkOrderItemNeeds> WorkOrderItemNeeds { get; set; }

        [InverseProperty("Item")]
        public virtual ICollection<ItemSerial> ItemSerial { get; set; }

        [InverseProperty("Item")]
        public virtual ICollection<WorkOrderDetail> WorkOrderDetail { get; set; }

        [InverseProperty("Item")]
        public virtual ICollection<ProductWastage> ProductWastage { get; set; }

        [InverseProperty("Item")]
        public virtual ICollection<ItemPrice> ItemPrice { get; set; }

        [InverseProperty("Item")]
        public virtual ICollection<EntryQualityData> EntryQualityData { get; set; }

        [InverseProperty("Item")]
        public virtual ICollection<FabricRecipe> FabricRecipe { get; set; }

        [InverseProperty("Item")]
        public virtual ICollection<ItemKnitDensity> ItemKnitDensity { get; set; }

        [InverseProperty("Item")]
        public virtual ICollection<KnitYarn> KnitYarn { get; set; }
    }
}
