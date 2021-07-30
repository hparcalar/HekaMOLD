using HekaMOLD.Business.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Order
{
    public class ItemOrderDetailModel : IDataObject
    {
        public int Id { get; set; }
        public int? ItemOrderId { get; set; }
        public int? LineNumber { get; set; }
        public int? ItemId { get; set; }
        public int? UnitId { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? NetQuantity { get; set; }
        public decimal? GrossQuantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public int? ForexId { get; set; }
        public decimal? ForexRate { get; set; }
        public decimal? ForexUnitPrice { get; set; }
        public bool? TaxIncluded { get; set; }
        public int? TaxRate { get; set; }
        public decimal? TaxAmount { get; set; }
        public int? DiscountRate { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? OverallTotal { get; set; }
        public string Explanation { get; set; }
        public int? OrderStatus { get; set; }
        public int? SyncStatus { get; set; }
        public DateTime? SyncDate { get; set; }
        public int? ItemRequestDetailId { get; set; }

        #region HEADER DATA
        public int? FirmId { get; set; }
        public string FirmCode { get; set; }
        public string FirmName { get; set; }
        public string OrderNo { get; set; }
        public DateTime? OrderDate { get; set; }
        public string OrderDateStr { get; set; }
        public string OrderExplanation { get; set; }
        #endregion

        #region VISUAL ELEMENTS
        public string ItemNo { get; set; }
        public string ItemName { get; set; }
        public string UnitCode { get; set; }
        public string UnitName { get; set; }
        public bool NewDetail { get; set; }
        #endregion

        #region PLANNING ELEMENTS
        public int? MachineId { get; set; }
        #endregion
    }
}
