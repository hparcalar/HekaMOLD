using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context.Models
{
 public   class District
    {
        public District()
        {

        }
        public int Id { get; set; }
        public string DistrictName { get; set; }
        [ForeignKey("City")]
        public int CityId { get; set; }

        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }

        public virtual City City { get; set; }

    }
}
