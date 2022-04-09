using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Core
{
    public class ItemQualityGroupModel
    {
        public int Id { get; set; }
        public string ItemQualityGroupCode { get; set; }
        public string ItemQualityGroupName { get; set; }
        public Nullable<int> PlantId { get; set; }
    }
}
