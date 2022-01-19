using System;

namespace Heka.DataAccess.Context.Models
{
   public partial class FirmTariff
    {
        public FirmTariff()
        {
                
        }
        public int Id { get; set; }
        public decimal? Ladametre { get; set; }
        public decimal? MeetrCup { get; set; }
        public decimal? Weight { get; set; }
        public int? ForexTypeId { get; set; }
        public int FirmId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }
        public Nullable<bool> IsActive { get; set; }

        public virtual Firm Firm { get; set; }
        public virtual ForexType ForexType { get; set; }


    }
}
