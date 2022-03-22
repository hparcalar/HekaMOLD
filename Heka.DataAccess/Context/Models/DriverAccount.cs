using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context
{
    public partial class DriverAccount
    {
        public DriverAccount()
        {
            this.DriverAccountDetail = new HashSet<DriverAccountDetail>();
        }
        public int Id { get; set; }
        [ForeignKey("Driver")]
        public int? DriverId { get; set; }
        [ForeignKey("ForexType")]
        public int? ForexTypeId { get; set; }
        public decimal? Balance { get; set; }

        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }

        public virtual Driver Driver { get; set; }
        public virtual ForexType ForexType { get; set; }

        [InverseProperty("DriverAccount")]
        public virtual ICollection<DriverAccountDetail> DriverAccountDetail { get; set; }
    }
}
