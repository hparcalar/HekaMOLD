using HekaMOLD.Business.Base;

namespace HekaMOLD.Business.Models.DataTransfer.Logistics
{
  public  class VehicleCareTypeModel : IDataObject
    {
        public int Id { get; set; }
        public string VehicleCareTypeCode { get; set; }
        public string VehicleCareTypeName { get; set; }
        public int? PeriyodUnitCode { get; set; }
        //Zorunlu
        public bool? Compulsory { get; set; }
    }
}
