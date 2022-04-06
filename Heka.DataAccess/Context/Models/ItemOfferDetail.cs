using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context
{
    public partial class ItemOfferDetail
    {
        public ItemOfferDetail()
        {
            this.ItemOfferDetailRoutePricing = new HashSet<ItemOfferDetailRoutePricing>();
            this.ItemOrderDetail = new HashSet<ItemOrderDetail>();
            this.ItemOfferSheetUsage = new HashSet<ItemOfferSheetUsage>();
        }
        public int Id { get; set; }

        [ForeignKey("ItemOffer")]
        public Nullable<int> ItemOfferId { get; set; }
        
        [ForeignKey("Item")]
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
        public Nullable<int> SheetTickness { get; set; }

        [ForeignKey("ItemOfferSheet")]
        public Nullable<int> ItemOfferSheetId { get; set; }

        [ForeignKey("Route")]
        public Nullable<int> RouteId { get; set; }

        public virtual ItemOffer ItemOffer { get; set; }
        public virtual Route Route { get; set; }
        public virtual Item Item { get; set; }

        public virtual ItemOfferSheet ItemOfferSheet { get; set; }

        [InverseProperty("ItemOfferDetail")]
        public virtual ICollection<ItemOfferDetailRoutePricing> ItemOfferDetailRoutePricing { get; set; }

        [InverseProperty("ItemOfferDetail")]
        public virtual ICollection<ItemOrderDetail> ItemOrderDetail { get; set; }

        [InverseProperty("ItemOfferDetail")]
        public virtual ICollection<ItemOfferSheetUsage> ItemOfferSheetUsage { get; set; }

    }
}
