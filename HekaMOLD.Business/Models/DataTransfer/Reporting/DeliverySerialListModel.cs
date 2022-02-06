using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Reporting
{
    public class DeliverySerialListModel
    {
        public int Id { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? NetWeight { get; set; }
        public decimal? GrossWeight { get; set; }
        public string ReceiptDate { get; set; }
        public string ReceiverText { get; set; }
        public string LineExplanation { get; set; }
        public string ReceiptExplanation { get; set; }

        #region HEADER (RECEIPT) INFORMATION
        public string FirmCode { get; set; }
        public string FirmName { get; set; }
        public string DocumentNo { get; set; }
        #endregion
    }
}
