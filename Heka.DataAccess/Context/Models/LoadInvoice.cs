using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context
{
    public class LoadInvoice
    {
        public LoadInvoice()
        {

        }
        public int Id { get; set; }
        public int InvoiceType { get; set; }
        [ForeignKey("Firm")]
        public int FirmId { get; set; }
        [ForeignKey("ItemLoad")]
        public int ItemLoadId { get; set; }
        [ForeignKey("ServiceItem")]
        public int ServiceItemId { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public decimal OverallTotal { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public int TaxRate { get; set; }
        public bool TaxIncluded { get; set; }
        public int ForexRate { get; set; }
        [ForeignKey("ForexType")]
        public int ForexId { get; set; }
        public string DocumentNo { get; set; }
        public int? IntegrationId { get; set; }
        public bool? Integration { get; set; }
        public string Explanation { get; set; }

        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }

        public virtual Firm Firm { get; set; }
        public virtual ItemLoad ItemLoad { get; set; }
        public virtual ServiceItem ServiceItem { get; set; }
        public virtual ForexType ForexType { get; set; }

    }
}
