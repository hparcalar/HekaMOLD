using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.Virtual
{
    public class ContractReceivingModel
    {
        public int? WorkOrderDetailId { get; set; }
        public int? DeliveredDetailId { get; set; }
        public decimal? Quantity { get; set; }
        public int? WarehouseId { get; set; }
        public string EntryDate { get; set; }
        public string DocumentNo { get; set; }
    }
}
