using HekaMOLD.Business.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Core
{
    public class ItemModel : IDataObject
    {
        public int Id { get; set; }
        public string ItemNo { get; set; }
        public string ItemName { get; set; }
        public int? ItemType { get; set; }
        public int? ItemCategoryId { get; set; }
        public int? ItemGroupId { get; set; }
        public int? SupplierFirmId { get; set; }
        public int? PlantId { get; set; }
        public int? MoldId { get; set; }
        public ItemWarehouseModel[] Warehouses { get; set; }
        public ItemUnitModel[] Units { get; set; }
        public ItemLiveStatusModel[] LiveStatus { get; set; }
        public decimal? TotalInQuantity { get; set; }
        public decimal? TotalOutQuantity { get; set; }
        public decimal? TotalOverallQuantity { get; set; }
        public bool? IsActive { get; set; }

        #region VISUAL ELEMENTS
        public string ItemTypeStr { get; set; }
        public string CategoryName { get; set; }
        public string GroupName { get; set; }
        #endregion
    }
}
