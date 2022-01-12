using HekaMOLD.Business.Base;

namespace HekaMOLD.Business.Models.DataTransfer.Logistics
{
    public class VehicleInsuranceTypeModel : IDataObject
    {
        public int Id { get; set; }
        public string VehicleInsuranceTypeCode { get; set; }
        public string VehicleInsuranceTypeName { get; set; }
        public int? PeriyodUnitid { get; set; }
        //Zorunlu
        public bool? Compulsory { get; set; }

        #region VISUAL ELEMENTS
        public string PeriyodUnitCode { get; set; }
        #endregion
    }
}
