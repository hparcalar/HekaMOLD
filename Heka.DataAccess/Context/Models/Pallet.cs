using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context
{
    public partial class Pallet
    {
        public Pallet()
        {

        }

        public int Id { get; set; }
        public string PalletNo { get; set; }
        public int PalletStatus { get; set; }

        [ForeignKey("Plant")]
        public Nullable<int> PlantId { get; set; }

        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }

        public virtual Plant Plant { get; set; }
    }
}
