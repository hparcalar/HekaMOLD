using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Core
{
    public class ItemWarehouseModel
    {
        public int Id { get; set; }
        public int? ItemId { get; set; }
        public int? WarehouseId { get; set; }
        public bool? IsAllowed { get; set; }
        public decimal? MinimumQuantity { get; set; }
        public decimal? MaximumQuantity { get; set; }
        public int? MinimumBehaviour { get; set; }
        public int? MaximumBehaviour { get; set; }

        #region VISUAL ELEMENTS
        public string WarehouseCode { get; set; }
        public string WarehouseName { get; set; }
        #endregion
    }
}
