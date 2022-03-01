using HekaMOLD.Business.Base;

namespace HekaMOLD.Business.Models.DataTransfer.Core
{
    public class FirmAddressModel : IDataObject
    {

        public int Id { get; set; }
        public string AddressName { get; set; }
        public int FirmId { get; set; }
        public int? CityId { get; set; }
        public int? CountryId { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string MobilePhone { get; set; }
        public string Fax { get; set; }
        public string OfficePhone { get; set; }
        public string AuthorizedInfo { get; set; }
        public int? AddressType { get; set; }

        #region VISUAL ELEMENYS

        public string FirmCode { get; set; }
        public string FirmName { get; set; }
        public string CityName { get; set; }
        public string CountryName { get; set; }
        public bool NewDetail { get; set; }
        #endregion
    }
}