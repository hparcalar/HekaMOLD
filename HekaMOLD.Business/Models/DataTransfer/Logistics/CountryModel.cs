using HekaMOLD.Business.Base;

namespace HekaMOLD.Business.Models.DataTransfer.Logistics
{
    public class CountryModel : IDataObject
    {
        public int Id { get; set; }
        public string DoubleCode { get; set; }
        public string ThreeCode { get; set; }
        public string CountryName { get; set; }
        public string NumberCode { get; set; }
    }
}
