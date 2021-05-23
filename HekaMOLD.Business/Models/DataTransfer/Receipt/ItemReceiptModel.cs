using HekaMOLD.Business.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Receipt
{
    public class ItemReceiptModel : IDataObject
    {
        public int Id { get; set; }
        public string ReceiptNo { get; set; }
        public string DocumentNo { get; set; }
        public DateTime? ReceiptDate { get; set; }
        public int? ReceiptType { get; set; }
        public int? FirmId { get; set; }
        public int? InWarehouseId { get; set; }
        public int? OutWarehouseId { get; set; }
        public int? PlantId { get; set; }
        public string Explanation { get; set; }
        public int? ReceiptStatus { get; set; }
        public int? ItemOrderId { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? TaxPrice { get; set; }
        public decimal? OverallTotal { get; set; }
        public int? SyncStatus { get; set; }
        public int? SyncPointId { get; set; }
        public DateTime? SyncDate { get; set; }
        public int? SyncUserId { get; set; }
        public ItemReceiptDetailModel[] Details { get; set; }

        #region VISUAL ELEMENTS
        public string FirmCode { get; set; }
        public string FirmName { get; set; }
        public string WarehouseCode { get; set; }
        public string WarehouseName { get; set; }
        public string ReceiptStatusStr { get; set; }
        public string CreatedDateStr { get; set; }
        public string ReceiptDateStr { get; set; }
        #endregion
    }
}
