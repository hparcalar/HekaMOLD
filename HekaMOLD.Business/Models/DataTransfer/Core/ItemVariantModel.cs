using HekaMOLD.Business.Base;
using HekaMOLD.Business.Models.DataTransfer.Production;
using System;

namespace HekaMOLD.Business.Models.DataTransfer.Core
{
    public class ItemVariantModel : IDataObject
    {
        public int Id { get; set; }
        public string ItemNo { get; set; }
        public string ItemName { get; set; }
        public Nullable<int> ItemType { get; set; }
        public Nullable<int> ItemCategoryId { get; set; }
        public Nullable<int> ItemGroupId { get; set; }
        public Nullable<int> SupplierFirmId { get; set; }
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

        public int? WeavingDraftId { get; set; }

        public int? ItemQualityTypeId { get; set; }

        public int? ItemId { get; set; }

        public ItemUnitModel[] Units { get; set; }
        public KnitYarnModel[] KnitYarns { get; set; }

        #region VISUAL ELEMENTS
        public string ItemTypeStr { get; set; }
        public string ItemCutTypeStr { get; set; }
        public string ItemBulletTypeStr { get; set; }
        public string ItemApparelTypeStr { get; set; }
        public string ItemDyeHouseTypeStr { get; set; }
        public string CategoryName { get; set; }
        public string GroupName { get; set; }
        public string QualityTypeName { get; set; }
        public string WeavingDraftCode { get; set; }
        #endregion
    }
}
