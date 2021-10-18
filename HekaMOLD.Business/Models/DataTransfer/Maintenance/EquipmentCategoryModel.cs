using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Maintenance
{
    public class EquipmentCategoryModel
    {
        public int Id { get; set; }
        public string EquipmentCategoryCode { get; set; }
        public string EquipmentCategoryName { get; set; }
        public int? PlantId { get; set; }
        public bool? IsCritical { get; set; }
    }
}
