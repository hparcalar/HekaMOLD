using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context
{
    public class ItemOfferSheet
    { 
        public ItemOfferSheet()
        {
            this.ItemOfferDetail = new HashSet<ItemOfferDetail>();
            this.ItemOfferSheetUsage = new HashSet<ItemOfferSheetUsage>();
        }

        public int Id { get; set; }

        [ForeignKey("ItemOffer")]
        public Nullable<int> ItemOfferId { get; set; }

        public string SheetName { get; set; }
        public Nullable<int> SheetNo { get; set; }
        public byte[] SheetVisual { get; set; }
        public Nullable<DateTime> PerSheetTime { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<decimal> Thickness { get; set; }
        public Nullable<decimal> SheetWidth { get; set; }
        public Nullable<decimal> SheetHeight { get; set; }
        public Nullable<decimal> Eff { get; set; }

        [ForeignKey("SheetItem")]
        public Nullable<int> SheetItemId { get; set; }
        public virtual ItemOffer ItemOffer { get; set; }
        public virtual Item SheetItem { get; set; }

        [InverseProperty("ItemOfferSheet")]
        public virtual ICollection<ItemOfferDetail> ItemOfferDetail { get; set; }

        [InverseProperty("ItemOfferSheet")]
        public virtual ICollection<ItemOfferSheetUsage> ItemOfferSheetUsage { get; set; }
    }
}
