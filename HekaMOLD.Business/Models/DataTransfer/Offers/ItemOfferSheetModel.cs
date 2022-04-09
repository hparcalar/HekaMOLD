using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Offers
{
    public class ItemOfferSheetModel
    {
        public int Id { get; set; }
        public Nullable<int> ItemOfferId { get; set; }
        public string SheetName { get; set; }
        public Nullable<int> SheetNo { get; set; }
        public byte[] SheetVisual { get; set; }
        public Nullable<DateTime> PerSheetTime { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<int> Thickness { get; set; }
        public Nullable<decimal> Eff { get; set; }
        public Nullable<int> SheetItemId { get; set; }

        #region VISUAL ELEMENTS
        public string SheetVisualStr { get; set; }
        #endregion
    }
}
