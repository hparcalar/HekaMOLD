using HekaMOLD.Business.Base;

namespace HekaMOLD.Business.Models.DataTransfer.Logistics
{
   public class CustomsModel : IDataObject
    {
        public int Id { get; set; }
        public string CustomsCode { get; set; }
        public string CustomsName { get; set; }
        public int? CityId { get; set; }

        public string CityName { get; set; }
        public string PostCode { get; set; }
        public string  CountryName { get; set; }
    }
}
