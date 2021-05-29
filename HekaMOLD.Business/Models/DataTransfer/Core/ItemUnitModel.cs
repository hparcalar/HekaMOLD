using HekaMOLD.Business.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Core
{
    public class ItemUnitModel : IDataObject
    {
        public int Id { get; set; }
        public int? ItemId { get; set; }
        public int? UnitId { get; set; }
        public bool? IsMainUnit { get; set; }
        public decimal? MultiplierFactor { get; set; }
        public decimal? DividerFactor { get; set; }

        #region VISUAL ELEMENTS
        public string UnitCode { get; set; }
        public string UnitName { get; set; }
        public bool NewDetail { get; set; }
        #endregion
    }
}
