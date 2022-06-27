using HekaMOLD.Business.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Lojistics
{
   public class LogisticsOrderModel : IDataObject
    {
        public int Id { get; set; }
        public string OrderCode { get; set; }
        public string DocumentNo { get; set; }
        public int? PlantId { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? UploadDate { get; set; }
        public DateTime? DeadlineDate { get; set; }
        public string UploadTypeCode { get; set; }
        public string TransactionDirectionCode { get; set; }
        public string StatusCode { get; set; }
        public string UploadPointCode { get; set; }
        public int? CustomerFirmId { get; set; }
        public int? ShipperFirmId { get; set; }
        public int? BuyerFirmId { get; set; }
        public decimal? TotalWeight { get; set; }
        public decimal? TotalVolume { get; set; }
        public decimal? Price { get; set; }
        public decimal? TotalPrice { get; set; }
        public string WeightUnitCode { get; set; }
        public string VolumeUnitCode { get; set; }
        public decimal? Amount { get; set; }
        public decimal? TotalAmount { get; set; }
        public string CurrencyCode { get; set; }
        public bool? Closed { get; set; }
        public bool? Approval { get; set; }
        public bool? Cancel { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public string InvoiceStatusCode { get; set; }
        public string ExitCustomsCode { get; set; }
        public string EntryCustomsCode { get; set; }

        public int? Rate { get; set; }
        public DateTime? RateDate { get; set; }

        public string CustomerFirmCode { get; set; }
        public string CustomerFirName { get; set; }
        public string ShipperFirmCode { get; set; }
        public string ShipperFirmName { get; set; }
        public string BuyerFirmCode { get; set; }
        public string BuyerFirmName { get; set; }
        public string OrderDateStr { get; set; }
        public string DeadlineDateStr { get; set; }
        public string UploadDateStr { get; set; }
    }
}
