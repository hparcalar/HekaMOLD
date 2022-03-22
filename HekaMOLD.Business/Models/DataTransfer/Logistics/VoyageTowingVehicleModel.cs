using HekaMOLD.Business.Base;
using System;

namespace HekaMOLD.Business.Models.DataTransfer.Logistics
{
    public class VoyageTowingVehicleModel : IDataObject
    {
        public int Id { get; set; }
        public int? TowingVehicleId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? StartKmHour { get; set; }
        public int? EndKmHour { get; set; }
        public int? DriverId { get; set; }

        public string DriverNameAndSurName { get; set; }
        public string StartDateStr { get; set; }
        public string EndDateStr { get; set; }
        public string TowingVehiclePlate { get; set; }
        public string TowingVehicleMark { get; set; }
        public string TowingVehicleVersiyon { get; set; }
        public string NewDetail { get; set; }
    }
}
