using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context.Models
{
    public partial class LogisticsOrderProductDetail
    {
        public LogisticsOrderProductDetail()
        {

        }
        public int Id { get; set; }

        [ForeignKey("Item")]
        public int ItemId { get; set; }

        [ForeignKey("LojisticsOrder")]
        public int LojisticsOrderId { get; set; }
        public Nullable<int> ShortWidth { get; set; }
        public Nullable<int> LongWidth { get; set; }
        public Nullable<decimal> Volume { get; set; }
        public Nullable<int> Height { get; set; }
        public Nullable<int> Weight { get; set; }
        public string VolumeUnitCode { get; set; }
        public string WeightUnitCode { get; set; }
        //istiflenebilir
        public bool? Stackable { get; set; }
        //Miktar
        public Nullable<int> Quantity { get; set; }
        public string UnitCode { get; set; }
        // Hesaplama türü
        public string CalculationType { get; set; }
        public Nullable<decimal> Dhl { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }
        public string CurrencyCode { get; set; }


    }
}
