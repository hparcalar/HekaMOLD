using HekaMOLD.Business.Base;
using HekaMOLD.Business.Models.DataTransfer.Production;

namespace HekaMOLD.Business.Models.DataTransfer.Core
{
    public class ItemModel : IDataObject
    {
        public int Id { get; set; }
        public string ItemNo { get; set; }
        public string ItemName { get; set; }
        public int? ItemType { get; set; }
        public int? ItemCutType { get; set; }
        public int? ItemApparelType { get; set; }
        public int? ItemBulletType { get; set; }
        public int? ItemDyeHouseType { get; set; }
        public int? ItemCategoryId { get; set; }
        public int? ItemGroupId { get; set; }
        public int? SupplierFirmId { get; set; }
        public int? PlantId { get; set; }
        public int? MoldId { get; set; }
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
        public int? TestNo { get; set; }
        //Tarak En
        public decimal? CombWidth { get; set; }
        //Atki Rapor Boyu
        public decimal? WeftReportLength { get; set; }
        //Cozgu Rapor Boyu
        public decimal? WarpReportLength { get; set; }
        //Atki Sikligi
        public int? WeftDensity { get; set; }

        public int? MachineId { get; set; }

        public int? ItemQualityTypeId { get; set; }

        public ItemWarehouseModel[] Warehouses { get; set; }
        public ItemUnitModel[] Units { get; set; }
        public ItemLiveStatusModel[] LiveStatus { get; set; }
        public KnitYarnModel[] KnitYarns { get; set; }

        public decimal? TotalInQuantity { get; set; }
        public decimal? TotalOutQuantity { get; set; }
        public decimal? TotalOverallQuantity { get; set; }

        #region VISUAL ELEMENTS
        public string ItemTypeStr { get; set; }
        public string ItemCutTypeStr { get; set; }
        public string ItemBulletTypeStr { get; set; }
        public string ItemApparelTypeStr { get; set; }
        public string ItemDyeHouseTypeStr { get; set; }
        public string CategoryName { get; set; }
        public string GroupName { get; set; }
        public string MachineCode { get; set; }
        public string MachineName { get; set; }
        public string QualityTypeName { get; set; }
        #endregion
    }
}
