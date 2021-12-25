using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Offers
{
    public class ItemOfferDetailModel
    {
        public int Id { get; set; }
        public Nullable<int> ItemOfferId { get; set; }
        public Nullable<int> ItemId { get; set; }

        public string ItemExplanation { get; set; }
        public string QualityExplanation { get; set; }
        public byte[] ItemVisual { get; set; }
        public Nullable<decimal> Quantity { get; set; }
        public Nullable<decimal> UnitPrice { get; set; }
        public Nullable<decimal> TotalPrice { get; set; }

        public Nullable<decimal> SheetWeight { get; set; }
        public Nullable<decimal> LaborCost { get; set; }
        public Nullable<decimal> WastageWeight { get; set; }
        public Nullable<decimal> ProfitRate { get; set; }
        public Nullable<int> CreditMonths { get; set; }
        public Nullable<int> CreditRate { get; set; }

        #region VISUAL ELEMENTS
        public bool NewDetail { get; set; } = false;
        public string ItemVisualStr { get; set; }
        #endregion
    }
}
