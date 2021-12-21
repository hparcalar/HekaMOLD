using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.Virtual
{
    public class ContractDeliveryModel
    {
        public int? FirmId { get; set; }
        public int? EntryReceiptDetailId { get; set; }
        public decimal? Quantity { get; set; }
        public int? WorkOrderDetailId { get; set; }
        public string DeliveryDate { get; set; }
        public string DocumentNo { get; set; }
    }
}
