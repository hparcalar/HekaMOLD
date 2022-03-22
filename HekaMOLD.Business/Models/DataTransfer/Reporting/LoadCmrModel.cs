namespace HekaMOLD.Business.Models.DataTransfer.Reporting
{
    public class LoadCmrModel
    {
        public int Id { get; set; }
        public string LoadCode { get; set; }
        public string OveralWeight { get; set; }
        public string OveralQuantity { get; set; }
        public string CmrShipperFirmName { get; set; }
        public string CmrShipperCountry { get; set; }
        public string CmrShipperCity { get; set; }
        public string CmrShipperAddress { get; set; }
        public string CmrBuyerFirmName { get; set; }
        public string CmrBuyerCountry { get; set; }
        public string CmrBuyerCity { get; set; }
        public string CmrBuyerAddress { get; set; }
        public string VehicleTraillerPlate { get; set; }
        public string LoadingDateStr { get; set; }

    }
}
