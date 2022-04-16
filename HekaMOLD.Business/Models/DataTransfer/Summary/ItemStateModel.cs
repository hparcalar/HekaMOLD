namespace HekaMOLD.Business.Models.DataTransfer.Summary
{
    public class ItemStateModel
    {
        public int ItemId { get; set; }
        public int WarehouseId { get; set; }
        public string ItemNo { get; set; }
        public string ItemName { get; set; }
        public int? ShortWidth { get; set; }
        public int? LongWidth { get; set; }
        public decimal? Volume { get; set; }
        public int? Height { get; set; }
        public int? Weight { get; set; }
        public bool? Stackable { get; set; }
        public decimal? Ladametre { get; set; }
        public string WarehouseCode { get; set; }
        public string WarehouseName { get; set; }
        public decimal? InQty { get; set; }
        public decimal? OutQty { get; set; }
        public decimal? TotalQty { get; set; }
    }
}
