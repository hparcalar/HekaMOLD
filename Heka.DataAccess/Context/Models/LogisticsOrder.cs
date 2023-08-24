using Heka.DataAccess.Context.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heka.DataAccess.Context
{
   public partial class LogisticsOrder
    {
        public LogisticsOrder()
        {
            this.LogisticsOrderProductDetail = new HashSet<LogisticsOrderProductDetail>();

        }
        public int Id { get; set; }
        public string OrderCode { get; set; }
        public string DocumentNo { get; set; }
        public Nullable<System.DateTime> OrderDate { get; set; }
        public Nullable<System.DateTime> UploadDate { get; set; }
        public Nullable<System.DateTime> DeadlineDate { get; set; }
        public string UploadTypeCode { get; set; }
        //İşlem yön kod
        public Nullable<int> PlantId { get; set; }
        public string TransactionDirectionCode { get; set; }
        public string StatusCode { get; set; }
        //yükleme Noktası Kod
        public string UploadPointCode { get; set; }
        [ForeignKey("FirmCustomer")]
        public Nullable<int> CustomerFirmId { get; set; }
        //Gönderici Firma
        [ForeignKey("FirmShipper")]
        public Nullable<int> ShipperFirmId { get; set; }
        [ForeignKey("FirmBuyer")]
        public Nullable<int> BuyerFirmId { get; set; }
        //Toplam Ağırlık
        public Nullable<decimal> TotalWeight { get; set; }
        //Toplam Hacim
        public Nullable<decimal> TotalVolume { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<decimal> TotalPrice { get; set; }
        public string WeightUnitCode { get; set; }
        public string VolumeUnitCode { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }
        public string CurrencyCode { get; set; }
        public bool? Closed { get; set; }
        public bool? Approval { get; set; }
        public bool? Cancel { get; set; }
        public Nullable<System.DateTime> ApprovalDate { get; set; }
        //Fatura Durum Kod
        public string InvoiceStatusCode { get; set; }
        //Çıkış Gümrük Kod
        public string ExitCustomsCode { get; set; }
        public string EntryCustomsCode { get; set; }

        //Kur
        public Nullable<int> Rate { get; set; }
        public Nullable<System.DateTime> RateDate { get; set; }

        public virtual Firm CustomerFirm { get; set; }
        public virtual Firm ShipperFirm { get; set; }
        public virtual Firm BuyerFirm { get; set; }

        public virtual Plant Plant { get; set; }

        [InverseProperty("LogisticsOrderProductDetail")]
        public virtual ICollection<LogisticsOrderProductDetail> LogisticsOrderProductDetail { get; set; }

    }
}
