using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Maintenance
{
    public class EquipmentModel
    {
        public int Id { get; set; }
        public string EquipmentCode { get; set; }
        public string EquipmentName { get; set; }
        public int? PlantId { get; set; }
        public int? MachineId { get; set; }
        public string Manufacturer { get; set; }
        public string ModelNo { get; set; }
        public string SerialNo { get; set; }
        public string Location { get; set; }
        public int? ResponsibleUserId { get; set; }
        public int? EquipmentCategoryId { get; set; }
        public bool? IsCritical { get; set; }

        #region VISUAL ELEMENTS
        public string MachineCode { get; set; }
        public string MachineName { get; set; }
        public string UserCode { get; set; }
        public string UserName { get; set; }
        public bool NewDetail { get; set; }
        #endregion
    }
}
