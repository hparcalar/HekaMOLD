using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Core
{
    public class SystemPrinterModel
    {
        public int Id { get; set; }
        public string PrinterCode { get; set; }
        public string PrinterName { get; set; }
        public string AccessPath { get; set; }
        public int? PlantId { get; set; }
        public bool? IsActive { get; set; }
        public decimal? PageWidth { get; set; }
        public decimal? PageHeight { get; set; }
    }
}
