using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Production
{
    public class ItemOrderItemNeedsModel
    {
        public int Id { get; set; }
        public int? ItemOrderDetailId { get; set; }
        public int? ItemOrderId { get; set; }
        public int? ItemId { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? RemainingQuantity { get; set; }
        public decimal? TargetQuantity { get; set; }
        public decimal? WarehouseQuantity { get; set; }
        public DateTime? CalculatedDate { get; set; }

        #region VISUAL ELEMENTS
        public string ItemOrderNo { get; set; }
        public string ItemOrderDateStr { get; set; }
        public string NeedsDateStr { get; set; }
        public string CalculatedDateStr { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string ItemNo { get; set; }
        public string ItemName { get; set; }
        public decimal RecipeQuantity { get; set; }
        #endregion
    }
}
