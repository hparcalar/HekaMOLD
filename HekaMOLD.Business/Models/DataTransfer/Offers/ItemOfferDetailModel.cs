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
        public Nullable<decimal> RoutePrice { get; set; }
        public Nullable<decimal> TotalPrice { get; set; }

        public Nullable<decimal> SheetWeight { get; set; }
        public Nullable<decimal> LaborCost { get; set; }
        public Nullable<decimal> WastageWeight { get; set; }
        public Nullable<decimal> ProfitRate { get; set; }
        public Nullable<int> CreditMonths { get; set; }
        public Nullable<int> CreditRate { get; set; }
        public Nullable<int> RouteId { get; set; }
        public Nullable<decimal> SheetTickness { get; set; }
        public Nullable<int> ItemOfferSheetId { get; set; }

        #region VISUAL ELEMENTS
        public ItemOfferDetailRoutePricingModel[] ProcessList { get; set; }
        public ItemOfferSheetUsageModel[] Usages { get; set; }
        public int SheetNo { get; set; }
        public bool NewDetail { get; set; } = false;
        public string ItemNo { get; set; }
        public string ItemName { get; set; }
        public string ItemVisualStr { get; set; }
        public string SheetVisualStr { get; set; }
        public string RouteCode { get; set; }
        public string RouteName { get; set; }
        #endregion
    }
}
