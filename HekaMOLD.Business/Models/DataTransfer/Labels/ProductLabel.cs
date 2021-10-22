using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Labels
{
    public class ProductLabel
    {
        public int Id { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string CreatedDateStr { get; set; }
        public string ShiftName { get; set; }
        public string InPackageQuantity { get; set; }
        public string Weight { get; set; }
        public string FirmName { get; set; }
        public string BarcodeContent { get; set; }
        public byte[] BarcodeImage { get; set; }
    }
}
