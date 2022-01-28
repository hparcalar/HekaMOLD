using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context.Models
{
    public partial class Country
    {
        public Country()
        {
            this.City = new HashSet<City>();
            this.Firm = new HashSet<Firm>();
            this.ItemLoadByCountryShipper = new HashSet<ItemLoad>();
            this.ItemLoadByCountryBuyer = new HashSet<ItemLoad>();

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
    }
}
