using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heka.DataAccess.Context
{
   public partial class YarnColourGroup
    {
        public YarnColourGroup()
        {
            this.YarnColour = new HashSet<YarnColour>();
        }
        public int Id { get; set; }
        public string YarnColourGroupCode { get; set; }
        public string YarnColourGroupName { get; set; }
        public Nullable<int> PlantId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }


        [InverseProperty("YarnColourGroup")]
        public virtual ICollection<YarnColour> YarnColour { get; set; }
    }
}
