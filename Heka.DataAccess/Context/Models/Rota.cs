﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context
{
    public partial class Rota
    {
        public Rota()
        {
            this.VoyageDetail = new HashSet<VoyageDetail>();
        }
        public int Id { get; set; }

        [ForeignKey("CityStart")]
        public Nullable<int> CityStartId { get; set; }

        [ForeignKey("CityEnd")]
        public Nullable<int> CityEndId { get; set; }

        public int KmHour { get; set; }
        public byte[] ProfileImage { get; set; }

        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }

        public virtual City CityStart { get; set; }
        public virtual City CityEnd { get; set; }
        [InverseProperty("Rota")]
        public virtual ICollection<VoyageDetail> VoyageDetail { get; set; }
    }
}
