using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context
{
    public class Voyage
    {
        public Voyage()
        {

        }
        public int Id { get; set; }
        public string VoyageCode { get; set; }
        public Nullable<DateTime> VoyageDate { get; set; }
        public int? VoyageStatus { get; set; }
        public Nullable<DateTime> StartDate { get; set; }
        public Nullable<DateTime> LoadDate { get; set; }
        public Nullable<DateTime> EndDate { get; set; }
        public int? TraillerType { get; set; }
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
        public decimal? OverallGrossWeight { get; set; }
        public decimal? OverallLadametre { get; set; }
        public int? PositionKmHour { get; set; }
        public int? LoadImportantType { get; set; }
        public string RingCode { get; set; }
        public string DocumentNo { get; set; }
        public bool? HasOperation { get; set; }
        public string LoadAddress { get; set; }
        public string DischargeAddress { get; set; }
        public Nullable<DateTime> ClosedDate { get; set; }
        public Nullable<DateTime> VehicleExitDate { get; set; }
        public int? RouteKmHour { get; set; }
        public string RouteDefinition { get; set; }
        public Nullable<DateTime> EndTime { get; set; }
        public string Explanation { get; set; }

        [ForeignKey("User")]
        public Nullable<int> CreatedUserId { get; set; }

        [ForeignKey("StartCity")]
        public int? StartCityId { get; set; }

        [ForeignKey("EndCity")]
        public int? EndCityId { get; set; }

        [ForeignKey("LoadCity")]
        public int? LoadCityId { get; set; }

        [ForeignKey("DischargeCity")]
        public int? DischargeCityId { get; set; }

        [ForeignKey("DischargeCountry")]
        public int? DischargeCountryId { get; set; }

        [ForeignKey("LoadCountry")]
        public int? LoadCountryId { get; set; }

        [ForeignKey("LoadCustoms")]
        public int? LoadCustomsId { get; set; }

        [ForeignKey("DischargeCustoms")]
        public int? DischargeCustomsId { get; set; }

        [ForeignKey("CustomsDoorEntry")]
        public int? CustomsDoorEntryId { get; set; }

        [ForeignKey("CustomsDoorExit")]
        public int? CustomsDoorExitId { get; set; }

        [ForeignKey("CarrierFirm")]
        public int? CarrierFirmId { get; set; }

        [ForeignKey("Driver")]
        public int? DriverId { get; set; }

        [ForeignKey("TowinfVehicle")]
        public int? TowinfVehicleId { get; set; }

        [ForeignKey("TraillerVehicle")]
        public int? TraillerVehicleId { get; set; }


        public Nullable<int> PlantId { get; set; }
        public Nullable<int> SyncStatus { get; set; }
        public Nullable<int> SyncPointId { get; set; }
        public Nullable<System.DateTime> SyncDate { get; set; }
        public Nullable<int> SyncUserId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }
        public string SyncKey { get; set; }

        public virtual User User { get; set; }
        public virtual City StartCity { get; set; }
        public virtual City EndCity { get; set; }
        public virtual City LoadCity { get; set; }
        public virtual City DischargeCity { get; set; }
        public virtual Country LoadCountry { get; set; }
        public virtual Country DischargeCountry { get; set; }
        public virtual Customs LoadCustoms { get; set; }
        public virtual Customs DischargeCustoms { get; set; }
        public virtual CustomsDoor CustomsDoorEntry { get; set; }
        public virtual CustomsDoor CustomsDoorExit { get; set; }
        public virtual Firm CarrierFirm { get; set; }
        public virtual Driver Driver { get; set; }
        public virtual Vehicle TowinfVehicle { get; set; }
        public virtual Vehicle TraillerVehicle { get; set; }


    }
}
