using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.MoldTrace
{
    public class MoldMoveHistory
    {
        public int? ItemReceiptDetailId { get; set; }
        public int? ItemReceiptId { get; set; }
        public int? ReceiptCategory { get; set; }
        public int? InvoiceId { get; set; }
        public string FirmCode { get; set; }
        public string FirmName { get; set; }
        public DateTime? ReceiptDate { get; set; }
        public string ReceiptDateStr { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string InvoiceDateStr { get; set; }
        public int? ReceiptType { get; set; }
        public string ReceiptTypeStr { get; set; }
        public int? InvoiceType { get; set; }
        public string InvoiceTypeStr { get; set; }
        public string ReceiptNo { get; set; }
        public string ReceiptDocumentNo { get; set; }
        public string InvoiceNo { get; set; }
        public string InvoiceDocumentNo { get; set; }
        public string WarehouseCode { get; set; }
        public string WarehouseName { get; set; }
    }
}
