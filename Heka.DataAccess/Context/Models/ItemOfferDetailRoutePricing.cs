using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context
{
    public class ItemOfferDetailRoutePricing
    {
        public int Id { get; set; }

        [ForeignKey("ItemOfferDetail")]
        public Nullable<int> ItemOfferDetailId { get; set; }

        [ForeignKey("RouteItem")]
        public Nullable<int> RouteItemId { get; set; }
        public Nullable<decimal> UnitPrice { get; set; }

        [ForeignKey("ForexType")]
        public Nullable<int> ForexId { get; set; }
        public Nullable<decimal> TotalPrice { get; set; }

        public virtual ItemOfferDetail ItemOfferDetail { get; set; }
        public virtual RouteItem RouteItem { get; set; }
        public virtual ForexType ForexType { get; set; }
    }
}
