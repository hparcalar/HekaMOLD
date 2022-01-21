using HekaMOLD.Business.Base;

namespace HekaMOLD.Business.Models.DataTransfer.Logistics
{
    public class CityModel : IDataObject
    {
        public int Id { get; set; }
        public string CityName { get; set; }
        public int CountryId { get; set; }
        public string PlateCode { get; set; }
        public string NumberCode { get; set; }
        public string PostCode { get; set; }
        public int? RowNumber { get; set; }

        #region VISUAL ELEMENTS
        public string CountryName { get; set; }
        #endregion
    }
}
