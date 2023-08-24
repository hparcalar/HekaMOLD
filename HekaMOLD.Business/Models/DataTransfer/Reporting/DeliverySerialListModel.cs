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
        public string Driver { get; set; }

        #region VISUAL ELEMENTS
        public int PackageCount { get; set; }
        public string PackageSize { get; set; }
        public int PalletCount { get; set; }
        #endregion
    }
}
