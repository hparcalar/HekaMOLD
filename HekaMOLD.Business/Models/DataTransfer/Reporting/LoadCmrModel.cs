namespace HekaMOLD.Business.Models.DataTransfer.Reporting
{
    public class LoadCmrModel
    {
        public int Id { get; set; }
        public string LoadCode { get; set; }
        public string OveralWeight { get; set; }
        public string OveralQuantity { get; set; }
        public string ShipperCountry { get; set; }
        public string ShipperCity { get; set; }
        public string ShipperAddress { get; set; }
        public string BuyerCountry { get; set; }
        public string BuyerCity { get; set; }
        public string BuyerAddress { get; set; }
    }
}
