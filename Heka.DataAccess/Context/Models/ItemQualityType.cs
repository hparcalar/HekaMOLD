﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heka.DataAccess.Context.Models
{
   public partial class ItemQualityType
    {
        public ItemQualityType()
        {
            this.Item = new HashSet<Item>();
        }
        public int Id { get; set; }
        public string ItemQualityTypeCode { get; set; }
        public string ItemQualityTypeName { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }

        [InverseProperty("Item")]
        public virtual ICollection<Item> Item { get; set; }

    }
}
