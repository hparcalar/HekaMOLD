using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Offers
{
    public class ItemOfferSheetUsageModel
    {
        public int Id { get; set; }
        public Nullable<int> ItemOfferDetailId { get; set; }
        public Nullable<int> ItemOfferSheetId { get; set; }
        public Nullable<int> Quantity { get; set; }

        #region VISUAL ELEMENTS
        public int SheetNo { get; set; }
        #endregion
    }
}
