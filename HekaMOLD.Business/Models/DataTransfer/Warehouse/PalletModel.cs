using HekaMOLD.Business.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Warehouse
{
    public class PalletModel : IDataObject
    {
        public int Id { get; set; }
        public string PalletNo { get; set; }
        public int PalletStatus { get; set; }
        public Nullable<int> PlantId { get; set; }

        #region VISUAL ELEMENTS
        public string CreatedDateStr { get; set; }
        public string ItemName { get; set; }
        public string FirmName { get; set; }
        public int BoxCount { get; set; }
        public decimal? Quantity { get; set; }
        public string QuantityStr { get; set; }
        public byte[] BarcodeImage { get; set; }
        #endregion
    }
}
