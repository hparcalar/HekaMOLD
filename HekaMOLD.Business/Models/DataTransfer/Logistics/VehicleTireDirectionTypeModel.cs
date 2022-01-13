using HekaMOLD.Business.Base;

namespace HekaMOLD.Business.Models.DataTransfer.Logistics
{
    public class VehicleTireDirectionTypeModel : IDataObject
    {
        public int Id { get; set; }
        public string VehicleTireDirectionTypeCode { get; set; }
        public string VehicleTireDirectionTypeName { get; set; }
    }
}
