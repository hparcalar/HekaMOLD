using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context
{
    public class ItemQualityGroup
    {
        public int Id { get; set; }
        public string ItemQualityGroupCode { get; set; }
        public string ItemQualityGroupName { get; set; }

        [ForeignKey("Plant")]
        public Nullable<int> PlantId { get; set; }

        public virtual Plant Plant { get; set; }
    }
}
