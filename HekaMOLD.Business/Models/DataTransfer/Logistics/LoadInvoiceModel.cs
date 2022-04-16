using HekaMOLD.Business.Base;
using System;

namespace HekaMOLD.Business.Models.DataTransfer.Logistics
{
    public class LoadInvoiceModel : IDataObject
    {
        public int Id { get; set; }
        public int InvoiceType { get; set; }
        public int FirmId { get; set; }
        public int ItemLoadId { get; set; }
        public int ServiceItemId { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public decimal OverallTotal { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public int TaxRate { get; set; }
        public bool TaxIncluded { get; set; }
        public int ForexRate { get; set; }
        public int ForexId { get; set; }
        public string DocumentNo { get; set; }
        public int? IntegrationId { get; set; }
        public bool? Integration { get; set; }
        public string Explanation { get; set; }

        #region VISUAL ELEMENTS
        public string InvoiceTypeStr { get; set; }
        public string FirmCode { get; set; }
        public string FirmName { get; set; }
        public string ServiceItemName { get; set; }
        public string InvoiceDateStr { get; set; }
        public string ForexCode { get; set; }
        public bool NewDetail { get; set; }
        #endregion
    }
}
