using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Core
{
    public class PrinterQueueModel
    {
        public int Id { get; set; }
        public int? PrinterId { get; set; }
        public int? RecordType { get; set; }
        public int? RecordId { get; set; }
        public int? OrderNo { get; set; }
        public string AllocatedPrintData { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
