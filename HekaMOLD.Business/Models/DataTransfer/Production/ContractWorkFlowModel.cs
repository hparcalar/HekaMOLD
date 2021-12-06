using HekaMOLD.Business.Models.DataTransfer.Receipt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Production
{
    public class ContractWorkFlowModel
    {
        public int Id { get; set; }

        public int? WorkOrderDetailId { get; set; }

        public int? DeliveredDetailId { get; set; }

        public int? ReceivedDetailId { get; set; }
        public DateTime? FlowDate { get; set; }
        public string FlowDateStr { get; set; }

        public string ItemNo { get; set; }
        public string ItemName { get; set; }
        public string FirmCode { get; set; }
        public string FirmName { get; set; }
        public decimal? Quantity { get; set; }
    }
}
