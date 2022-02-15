﻿using System;
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
            this.Customs = new HashSet<Customs>();
            this.Customs = new HashSet<Customs>();
            this.Firm = new HashSet<Firm>();
            this.ItemLoadByCityShipper = new HashSet<ItemLoad>();
            this.ItemLoadByCityBuyer = new HashSet<ItemLoad>();
            this.RotaByCityStart = new HashSet<Rota>();
            this.RotaByCityEnd = new HashSet<Rota>();
            this.VoyageByStartCity = new HashSet<Voyage>();
            this.VoyageByEndCity = new HashSet<Voyage>();
            this.VoyageByLoad = new HashSet<Voyage>();
            this.VoyageByDischarge = new HashSet<Voyage>();

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

        [InverseProperty("City")]
        public virtual ICollection<Customs> Customs { get; set; }

        [InverseProperty("LoadCity")]
        public virtual ICollection<ItemOrder> ItemOrderByLoad { get; set; }

        [InverseProperty("DischargeCity")]
        public virtual ICollection<ItemOrder> ItemOrderByDischarge { get; set; }

        [InverseProperty("City")]
        public virtual ICollection<Firm> Firm { get; set; }

        [InverseProperty("CityShipper")]
        public virtual ICollection<ItemLoad> ItemLoadByCityShipper { get; set; }

        [InverseProperty("CityBuyer")]
        public virtual ICollection<ItemLoad> ItemLoadByCityBuyer { get; set; }

        [InverseProperty("CityStart")]
        public virtual ICollection<Rota> RotaByCityStart { get; set; }

        [InverseProperty("CityEnd")]
        public virtual ICollection<Rota> RotaByCityEnd { get; set; }

        [InverseProperty("StartCity")]
        public virtual ICollection<Voyage> VoyageByStartCity { get; set; }

        [InverseProperty("EndCity")]
        public virtual ICollection<Voyage> VoyageByEndCity { get; set; }

        [InverseProperty("LoadCity")]
        public virtual ICollection<Voyage> VoyageByLoad { get; set; }

        [InverseProperty("DischargeCity")]
        public virtual ICollection<Voyage> VoyageByDischarge { get; set; }

    }
}
