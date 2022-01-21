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
    }
}
