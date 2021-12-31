using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Receipt
{
    public class CountingReceiptModel
    {
        public int Id { get; set; }
        public Nullable<DateTime> CountingDate { get; set; }
        public string ReceiptNo { get; set; }
        public Nullable<int> PlantId { get; set; }
        public Nullable<int> CountingStatus { get; set; }

        #region VISUAL ELEMENTS
        public CountingReceiptDetailModel[] Details { get; set; }
        public string CountingDateStr { get; set; }
        #endregion
    }
}
