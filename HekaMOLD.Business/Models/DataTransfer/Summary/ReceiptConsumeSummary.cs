using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Summary
{
    public class ReceiptConsumeSummary
    {
        public int ConsumedReceiptDetailId { get; set; }
        public int ConsumerLineNumber { get; set; }
        public decimal? UsedQuantity { get; set; }
    }
}
