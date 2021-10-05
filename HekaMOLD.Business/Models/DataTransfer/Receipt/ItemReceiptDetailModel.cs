using HekaMOLD.Business.Base;
using HekaMOLD.Business.Models.DataTransfer.Production;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Receipt
{
    public class ItemReceiptDetailModel : IDataObject
    {
        public int Id { get; set; }
        public int? ItemReceiptId { get; set; }
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
        public int? ReceiptStatus { get; set; }
        public int? SyncStatus { get; set; }
        public DateTime? SyncDate { get; set; }
        public int? ItemOrderDetailId { get; set; }

        #region SERIAL SAVING PARAMETERS
        public bool UpdateSerials { get; set; } = false;
        public List<WorkOrderSerialModel> Serials { get; set; } = new List<WorkOrderSerialModel>();
        #endregion

        #region VISUAL ELEMENTS
        public string ItemNo { get; set; }
        public string ItemName { get; set; }
        public string UnitCode { get; set; }
        public string UnitName { get; set; }
        public string FirmCode { get; set; }
        public string FirmName { get; set; }
        public string WarehouseCode { get; set; }
        public string WarehouseName { get; set; }
        public string ReceiptTypeStr { get; set; }
        public string ReceiptDateStr { get; set; }
        public decimal? InQuantity { get; set; }
        public decimal? OutQuantity { get; set; }
        public bool NewDetail { get; set; }
        #endregion
    }
}
