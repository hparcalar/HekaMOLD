namespace HekaMOLD.Business.Models.DataTransfer.Core
{
    public class ForexTypeModel
    {
        public int Id { get; set; }
        public string ForexTypeCode { get; set; }
        public decimal? ActiveSalesRate { get; set; }
        public decimal? ActiveBuyingRate { get; set; }
    }
}
