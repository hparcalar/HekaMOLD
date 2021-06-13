using HekaMOLD.Business.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Production
{
    public class ProductRecipeDetailModel : IDataObject
    {
        public int Id { get; set; }
        public int? ProductRecipeId { get; set; }
        public int? ItemId { get; set; }
        public int? ProcessType { get; set; }
        public int? UnitId { get; set; }
        public decimal? Quantity { get; set; }
        public int? WarehouseId { get; set; }
        public decimal? WastagePercentage { get; set; }

        #region VISUAL ELEMENTS
        public bool NewDetail { get; set; }
        public string ItemNo { get; set; }
        public string ItemName { get; set; }
        public string ProcessTypeStr { get; set; }
        public string UnitCode { get; set; }
        public string UnitName { get; set; }
        public string WarehouseCode { get; set; }
        public string WarehouseName { get; set; }
        #endregion
    }
}
