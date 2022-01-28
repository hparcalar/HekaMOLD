using HekaMOLD.Business.Base;
using System;

namespace HekaMOLD.Business.Models.DataTransfer.Logistics
{
    public class ItemLoadDetailModel : IDataObject
    {
        public int Id { get; set; }

        public Nullable<int> ItemLoadId { get; set; }
        public Nullable<int> LineNumber { get; set; }
        public Nullable<int> ItemId { get; set; }
        public Nullable<int> UnitId { get; set; }
        public Nullable<int> ItemOrderDetailId { get; set; }

        public Nullable<decimal> Quantity { get; set; }
        public Nullable<decimal> NetQuantity { get; set; }
        public Nullable<decimal> GrossQuantity { get; set; }
        public Nullable<decimal> UnitPrice { get; set; }
        public int? ShortWidth { get; set; }
        public int? LongWidth { get; set; }
        public decimal? Volume { get; set; }
        public int? Height { get; set; }
        public int? Weight { get; set; }
        public int? PackageInNumber { get; set; }
        //istiflenebilir
        public bool? Stackable { get; set; }
        public decimal? Ladametre { get; set; }
        public Nullable<int> LoadStatus { get; set; }

        #region VISUAL ELEMENTS
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string UnitTypeCode { get; set; }
        public bool NewDetail { get; set; }

        #endregion
    }
}
