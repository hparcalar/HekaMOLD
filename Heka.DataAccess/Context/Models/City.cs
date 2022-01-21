using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context.Models
{
    public partial class City
    {
        public City()
        {
            this.District =new HashSet<District>();
            this.ItemOrderByLoad = new HashSet<ItemOrder>();
            this.ItemOrderByDischarge = new HashSet<ItemOrder>();

        }
        public int Id { get; set; }
        public string CityName { get; set; }

        [ForeignKey("Country")]
        public int CountryId { get; set; }
        public string PlateCode { get; set; }
        public string NumberCode { get; set; }
        public string PostCode { get; set; }
        public int? RowNumber { get; set; }

        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }

        public virtual Country Country { get; set; }

        [InverseProperty("City")]
        public virtual ICollection<District> District { get; set; }

        [InverseProperty("LoadCity")]
        public virtual ICollection<ItemOrder> ItemOrderByLoad { get; set; }

        [InverseProperty("DischargeCity")]
        public virtual ICollection<ItemOrder> ItemOrderByDischarge { get; set; }
    }
}
