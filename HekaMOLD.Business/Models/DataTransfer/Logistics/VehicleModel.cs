using HekaMOLD.Business.Base;
using System;

namespace HekaMOLD.Business.Models.DataTransfer.Logistics

{
    public class VehicleModel : IDataObject
    {
        public int Id { get; set; }
        public string Plate { get; set; }
        public string Mark { get; set; }
       
        public string Versiyon { get; set; }
        public string ChassisNumber { get; set; }
        public int? VehicleAllocationType { get; set; }
        public int? VehicleTypeId { get; set; }
        public int? OwnerFirmId { get; set; }
        public Nullable<DateTime> ContractStartDate { get; set; }
        public Nullable<DateTime> ContractEndDate { get; set; }
        public int? KmHour { get; set; }
        public decimal? Price { get; set; }
        public int? UnitId { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }
        public decimal? TrailerHeadWeight { get; set; }
        public decimal? LoadCapacity { get; set; }
        public int? TrailerType { get; set; }
        public bool? StatusCode { get; set; }
        public int? CarePeriyot { get; set; }
        // Oransal sınır
        public int? ProportionalLimit { get; set; }
        // Bakım bildirim
        public bool? CareNotification { get; set; }
        //Lastik bildirim
        public bool? TireNotification { get; set; }
        public bool? Approval { get; set; }
        public bool? Invalidation { get; set; }
        public bool? KmHourControl { get; set; }
        public bool? HasLoadPlannig { get; set; }


        public VehicleCareModel[] VehicleCares { get; set; }
        public VehicleInsuranceModel[] VehicleInsurances { get; set; }
        public VehicleTireModel[] VehicleTires { get; set; }


        #region VISUAL ELEMENTS
        public String VehicleAllocationTypeStr { get; set; }
        public String VehicleTypeCode { get; set; }
        public String VehicleTypeName { get; set; }
        public String OwnerFirmCode { get; set; }
        public String OwnerFirmName { get; set; }
        public string ContractStartDateStr { get; set; }
        public string ContractEndDateStr { get; set; }
        public string UnitCode { get; set; }
        public string UnitName { get; set; }
        public string TrailerTypeStr { get; set; }
        #endregion

    }
}
