using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Summary
{
    public class ItemSerialSummary
    {
        public string ItemNo { get; set; }
        public string ItemName { get; set; }
        public int? SerialCount { get; set; }
        public decimal? SerialSum { get; set; }
        public string PartVisualStr { get; set; }
    }
}
