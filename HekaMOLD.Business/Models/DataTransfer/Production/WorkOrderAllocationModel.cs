using HekaMOLD.Business.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Production
{
    public class WorkOrderAllocationModel : IDataObject
    {
        public int Id { get; set; }
        public int? WorkOrderDetailId { get; set; }
        public int? ItemReceiptDetailId { get; set; }
        public decimal? Quantity { get; set; }
        public int? PackageQuantity { get; set; }

        #region VISUAL ELEMENTS
        public string WorkOrderNo { get; set; }
        public string WorkOrderDate { get; set; }
        public string ItemReceiptDocumentNo { get; set; }
        public string ItemReceiptNo { get; set; }
        public string ItemReceiptFirmCode { get; set; }
        public string ItemReceiptFirmName { get; set; }
        public string WorkOrderFirmCode { get; set; }
        public string WorkOrderFirmName { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        #endregion
    }
}
