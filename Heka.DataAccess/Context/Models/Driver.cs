using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context
{
   public partial class Driver
    {
        public Driver()
        {
            this.Voyage = new HashSet<Voyage>();
            this.ItemLoad = new HashSet<ItemLoad>();
            this.VoyageDriver = new HashSet<VoyageDriver>();
            this.VoyageTowingVehicle = new HashSet<VoyageTowingVehicle>();
            this.VoyageCostDetail = new HashSet<VoyageCostDetail>();
            this.DriverAccountDetail = new HashSet<DriverAccountDetail>();
            this.DriverAccount = new HashSet<DriverAccount>();

        }
        public int Id { get; set; }
        public string DriverName { get; set; }
        public string DriverSurName { get; set; }
        public string Tc { get; set; }
        public string PlaceOfBirth { get; set; }
        public DateTime?  BirthDate { get; set; }
        public string PassportNo { get; set; }
        [ForeignKey("Country")]
        public int? CountryId { get; set; }
        public int? VisaType { get; set; }
        public DateTime? VisaStartDate { get; set; }
        public DateTime? VisaEndDate { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }
        public byte[] ProfileImage { get; set; }

        public virtual Country Country { get; set; }

        [InverseProperty("Driver")]
        public virtual ICollection<Voyage> Voyage { get; set; }

        [InverseProperty("Driver")]
        public virtual ICollection<ItemLoad> ItemLoad { get; set; }

        [InverseProperty("Driver")]
        public virtual ICollection<VoyageDriver> VoyageDriver { get; set; }

        [InverseProperty("Driver")]
        public virtual ICollection<VoyageTowingVehicle> VoyageTowingVehicle { get; set; }

        [InverseProperty("Driver")]
        public virtual ICollection<VoyageCostDetail> VoyageCostDetail { get; set; }

        [InverseProperty("Driver")]
        public virtual ICollection<DriverAccountDetail> DriverAccountDetail { get; set; }

        [InverseProperty("Driver")]
        public virtual ICollection<DriverAccount> DriverAccount { get; set; }

    }
}
