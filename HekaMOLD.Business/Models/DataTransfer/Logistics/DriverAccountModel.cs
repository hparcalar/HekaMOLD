using HekaMOLD.Business.Base;

namespace HekaMOLD.Business.Models.DataTransfer.Logistics
{
    public class DriverAccountModel : IDataObject
    {
        public int Id { get; set; }
        public int? DriverId { get; set; }
        public int? ForexTypeId { get; set; }
        public decimal? Balance { get; set; }
        public DriverAccountDetailModel[] DriverAccountDetails { get; set; }

        public string DriverNameAndSurName { get; set; }
        public string ForexTypeCode { get; set; }
    }
}
