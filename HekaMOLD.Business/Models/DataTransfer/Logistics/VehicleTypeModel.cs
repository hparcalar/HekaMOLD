using HekaMOLD.Business.Base;

namespace HekaMOLD.Business.Models.DataTransfer.Logistics
{
    public class VehicleTypeModel : IDataObject
    {
        public int Id { get; set; }
        public string VehicleTypeCode { get; set; }
        public string VehicleTypeName { get; set; }
    }
}
