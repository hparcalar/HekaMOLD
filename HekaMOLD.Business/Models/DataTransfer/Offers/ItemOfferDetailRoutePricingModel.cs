using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Offers
{
    public class ItemOfferDetailRoutePricingModel
    {
        public int Id { get; set; }
        public Nullable<int> ItemOfferDetailId { get; set; }
        public Nullable<int> RouteItemId { get; set; }
        public Nullable<decimal> UnitPrice { get; set; }
        public Nullable<int> ForexId { get; set; }
        public Nullable<decimal> TotalPrice { get; set; }

        #region VISUAL ELEMENTS
        public string ForexCode { get; set; }
        public string ProcessCode { get; set; }
        public string ProcessName { get; set; }
        public string RouteCode { get; set; }
        public string RouteName { get; set; }
        #endregion
    }
}
