using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Summary
{
    public class SalesReportModel
    {
        public int ItemId { get; set; }
        public int? FirmId { get; set; }
        public int? ItemGroupId { get; set; }
        public string ItemNo { get; set; }
        public string ItemGroupName { get; set; }
        public string ItemName { get; set; }
        public string FirmName { get; set; }
        public decimal? Quantity { get; set; }
        public string ConsumptionItemName { get; set; }
        public decimal? ConsumptionWeight { get; set; }
    }
}
