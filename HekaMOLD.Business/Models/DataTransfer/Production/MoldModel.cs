using HekaMOLD.Business.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Production
{
    public class MoldModel : IDataObject
    {
        public int Id { get; set; }
        public string MoldCode { get; set; }
        public string MoldName { get; set; }
        public int? PlantId { get; set; }
        public bool? IsActive { get; set; }
        public int? FirmId { get; set; }
        public int? LifeTimeTicks { get; set; }
        public int? CurrentTicks { get; set; }
        public int? MoldItemId { get; set; }
        public DateTime? OwnedDate { get; set; }
        public int? MoldStatus { get; set; }
        public string Explanation { get; set; }
        public int? InWarehouseId { get; set; }
        public MoldProductModel[] Products { get; set; }

        #region VISUAL ELEMENTS
        public string CreatedDateStr { get; set; }
        public string OwnedDateStr { get; set; }
        public string FirmCode { get; set; }
        public string FirmName { get; set; }
        public string ItemNo { get; set; }
        public string ItemName { get; set; }
        public string MoldStatusText { get; set; }
        public string WarehouseCode { get; set; }
        public string WarehouseName { get; set; }
        #endregion
    }
}
