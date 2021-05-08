using HekaMOLD.Business.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Request
{
    public class ItemRequestDetailModel : IDataObject
    {
        public int Id { get; set; }
        public int? ItemRequestId { get; set; }
        public int? LineNumber { get; set; }
        public int? ItemId { get; set; }
        public int? UnitId { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? NetQuantity { get; set; }
        public decimal? ApprovedQuantity { get; set; }
        public string Explanation { get; set; }
        public int? RequestStatus { get; set; }

        #region VISUAL ELEMENTS
        public string ItemNo { get; set; }
        public string ItemName { get; set; }
        public string UnitCode { get; set; }
        public string UnitName { get; set; }
        public bool NewDetail { get; set; }
        #endregion
    }
}
