using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context
{
    public partial class VoyageDriver
    {
        public int Id { get; set; }
        [ForeignKey("Driver")]
        public int? DriverId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? StartKmHour { get; set; }
        public int? EndKmHour { get; set; }
        [ForeignKey("Vehicle")]
        public int? TowingVehicleId { get; set; }
        [ForeignKey("Voyage")]
        public int? VoyageId { get; set; }

        public virtual Voyage Voyage { get; set; }
        public virtual Driver Driver{ get; set; }
        public virtual Vehicle Vehicle { get; set; }

    }
}
