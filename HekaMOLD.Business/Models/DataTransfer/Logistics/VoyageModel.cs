using HekaMOLD.Business.Base;
using System;

namespace HekaMOLD.Business.Models.DataTransfer.Logistics
{
    public class VoyageModel : IDataObject
    {
        public int Id { get; set; }
        public string VoyageCode { get; set; }
        public Nullable<DateTime> VoyageDate { get; set; }
        public int? VoyageStatus { get; set; }
        public Nullable<DateTime> StartDate { get; set; }
        public Nullable<DateTime> LoadDate { get; set; }
        public Nullable<DateTime> EndDate { get; set; }
        public int? TraillerType { get; set; }
        public int? OrderTransactionDirectionType { get; set; }
        public int? UploadType { get; set; }
        public bool? HasTraillerTransportation { get; set; }
        public bool? EmptyGo { get; set; }
        public bool? HasNotRationCard { get; set; }
        public Nullable<DateTime> CustomsDoorEntryDate { get; set; }
        public Nullable<DateTime> CustomsDoorExitDate { get; set; }
        public string TraillerRationCardNo { get; set; }
        public Nullable<DateTime> TraillerRationCardClosedDate { get; set; }
        public decimal? OverallQuantity { get; set; }
        public decimal? OverallVolume { get; set; }
        public decimal? OverallWeight { get; set; }
        public decimal? OverallGrossWeight { get; set; }
        public decimal? OverallLadametre { get; set; }
        public int? PositionKmHour { get; set; }
        public int? LoadImportantType { get; set; }
        public string RingCode { get; set; }
        public string DocumentNo { get; set; }
        public bool? HasOperation { get; set; }
        public string StartAddress { get; set; }
        public string LoadAddress { get; set; }
        public string DischargeAddress { get; set; }
        public Nullable<DateTime> ClosedDate { get; set; }
        public Nullable<DateTime> VehicleExitDate { get; set; }
        public int? RouteKmHour { get; set; }
        public string RouteDefinition { get; set; }
        public Nullable<DateTime> EndTime { get; set; }
        public decimal? DriverSubsistence { get; set; }
        public Nullable<DateTime> FirstLoadDate { get; set; }
        public Nullable<DateTime> EndDischargeDate { get; set; }
        public Nullable<DateTime> KapikulePassportEntryDate { get; set; }
        public Nullable<DateTime> KapikulePassportExitDate { get; set; }

        public string Explanation { get; set; }

        public int? StartCityId { get; set; }
        public int? EndCityId { get; set; }
        public int? LoadCityId { get; set; }
        public int? DischargeCityId { get; set; }
        public int? StartCountryId { get; set; }
        public int? DischargeCountryId { get; set; }
        public int? LoadCountryId { get; set; }
        public int? ExitCustomsId { get; set; }
        public int? EntryCustomsId { get; set; }
        public int? CustomsDoorEntryId { get; set; }
        public int? CustomsDoorExitId { get; set; }
        public int? CarrierFirmId { get; set; }
        public int? DriverId { get; set; }
        public int? TowinfVehicleId { get; set; }
        public int? TraillerVehicleId { get; set; }
        public int? ForexTypeId { get; set; }


        public Nullable<int> PlantId { get; set; }
        public Nullable<int> SyncStatus { get; set; }
        public Nullable<int> SyncPointId { get; set; }
        public Nullable<System.DateTime> SyncDate { get; set; }
        public Nullable<int> SyncUserId { get; set; }
        public string SyncKey { get; set; }
        public VoyageDetailModel[] VoyageDetails { get; set; }
        public VoyageDriverModel[] VoyageDrivers { get; set; }
        public VoyageTowingVehicleModel[] VoyageTowingVehicles { get; set; }

        #region VISUAL ELEMENTS
        public string VoyageDateStr { get; set; }
        public string StartDateStr { get; set; }
        public string LoadDateStr { get; set; }
        public string EndDateStr { get; set; }
        public string TraillerTypeStr { get; set; }
        public string UploadTypeStr { get; set; }
        public string CustomsDoorEntryDateStr { get; set; }
        public string CustomsDoorExitDateStr { get; set; }
        public string TraillerRationCardClosedDateStr { get; set; }
        public string LoadImportantTypeStr { get; set; }
        public string ClosedDateStr { get; set; }
        public string VehicleExitDateStr { get; set; }
        public string StartCityPostCode { get; set; }
        public string StartCityName { get; set; }
        public string EndCityPostCode { get; set; }
        public string EndCityName { get; set; }
        public string LoadCityPostCode { get; set; }
        public string LoadCityName { get; set; }
        public string DischargeCityPostCode { get; set; }
        public string DischargeCityName { get; set; }
        public string StartCountryName { get; set; }
        public string DischargeCountryName { get; set; }
        public string LoadCountryName { get; set; }
        public string ExitCustomsName { get; set; }
        public string EntryCustomsName { get; set; }
        public string CustomsDoorEntryName { get; set; }
        public string CustomsDoorExitName { get; set; }
        public string CarrierFirmCode { get; set; }
        public string CarrierFirmName { get; set; }
        public string DriverNameAndSurname { get; set; }
        public string TowinfVehiclePlate { get; set; }
        public string TowinfVehicleMarkAndModel { get; set; }
        public string TraillerVehiclePlate { get; set; }
        public string TraillerVehicleMarkAndModel { get; set; }
        public string OrderTransactionDirectionTypeStr { get; set; }
        public string VoyageStatusStr { get; internal set; }
        public string ForexTypeCode { get; internal set; }
        public string FirstLoadDateStr { get; set; }
        public string EndDischargeDateStr { get; set; }
        public string KapikulePassportEntryDateStr { get; set; }
        public string KapikulePassportExitDateStr { get; set; }

        #endregion
    }
}
