using HekaMOLD.Business.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Finance
{
    public class InvoiceModel : IDataObject
    {
        public int Id { get; set; }
        public int? InvoiceType { get; set; }
        public string InvoiceNo { get; set; }
        public int? InvoiceStatus { get; set; }
        public string DocumentNo { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public int? FirmId { get; set; }
        public int? PlantId { get; set; }
        public string Explanation { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? DiscountTotal { get; set; }
        public decimal? TaxTotal { get; set; }
        public decimal? GrandTotal { get; set; }
    }
}
