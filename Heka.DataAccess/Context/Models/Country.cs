using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context
{
    public partial class Country
    {
        public Country()
        {
            this.City = new HashSet<City>();
            this.Firm = new HashSet<Firm>();
            this.ItemLoadByCountryShipper = new HashSet<ItemLoad>();
            this.ItemLoadByCountryBuyer = new HashSet<ItemLoad>();
            this.ItemLoadByCountryCmrShipper = new HashSet<ItemLoad>();
            this.ItemLoadByCountryCmrBuyer = new HashSet<ItemLoad>();
            this.Driver = new HashSet<Driver>();
            this.VoyageByStartCountry = new HashSet<Voyage>();
            this.VoyageByLoadCountry = new HashSet<Voyage>();
            this.VoyageByDischargeCountry = new HashSet<Voyage>();
            this.VoyageDetailByCountryShipper = new HashSet<VoyageDetail>();
            this.VoyageDetailByCountryBuyer = new HashSet<VoyageDetail>();
            this.FirmAddress = new HashSet<FirmAddress>();
            this.ItemLoadByVoyageStartCountry = new HashSet<ItemLoad>();
            this.ItemLoadByVoyageEndCountry = new HashSet<ItemLoad>();
            this.VoyageCostDetail = new HashSet<VoyageCostDetail>();
            this.DriverAccountDetail = new HashSet<DriverAccountDetail>();

        }
        public int Id { get; set; }
        public string DoubleCode { get; set; }
        public string ThreeCode { get; set; }
        public string CountryName { get; set; }
        public string NumberCode { get; set; }

        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }

        [InverseProperty("Country")]
        public virtual ICollection<City> City { get; set; }

        [InverseProperty("Country")]
        public virtual ICollection<Firm> Firm { get; set; }

        [InverseProperty("CountryShipper")]
        public virtual ICollection<ItemLoad> ItemLoadByCountryShipper { get; set; }

        [InverseProperty("CountryBuyer")]
        public virtual ICollection<ItemLoad> ItemLoadByCountryBuyer { get; set; }

        [InverseProperty("CountryCmrShipper")]
        public virtual ICollection<ItemLoad> ItemLoadByCountryCmrShipper { get; set; }

        [InverseProperty("CountryCmrBuyer")]
        public virtual ICollection<ItemLoad> ItemLoadByCountryCmrBuyer { get; set; }

        [InverseProperty("Country")]
        public virtual ICollection<Driver> Driver { get; set; }

        [InverseProperty("StartCountry")]
        public virtual ICollection<Voyage> VoyageByStartCountry { get; set; }

        [InverseProperty("LoadCountry")]
        public virtual ICollection<Voyage> VoyageByLoadCountry { get; set; }

        [InverseProperty("DischargeCountry")]
        public virtual ICollection<Voyage> VoyageByDischargeCountry { get; set; }

        [InverseProperty("CountryShipper")]
        public virtual ICollection<VoyageDetail> VoyageDetailByCountryShipper { get; set; }

        [InverseProperty("CountryBuyer")]
        public virtual ICollection<VoyageDetail> VoyageDetailByCountryBuyer { get; set; }

        [InverseProperty("Country")]
        public virtual ICollection<FirmAddress> FirmAddress { get; set; }

        [InverseProperty("VoyageStartCountry")]
        public virtual ICollection<ItemLoad> ItemLoadByVoyageStartCountry { get; set; }

        [InverseProperty("VoyageEndCountry")]
        public virtual ICollection<ItemLoad> ItemLoadByVoyageEndCountry { get; set; }

        [InverseProperty("Country")]
        public virtual ICollection<VoyageCostDetail> VoyageCostDetail { get; set; }

        [InverseProperty("Country")]
        public virtual ICollection<DriverAccountDetail> DriverAccountDetail { get; set; }
    }
}
