using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context
{
    public partial class ItemOffer
    {
        public ItemOffer()
        {
            this.ItemOfferDetail = new HashSet<ItemOfferDetail>();
        }

        public int Id { get; set; }
        public string OfferNo { get; set; }
        public int OfferType { get; set; }

        [ForeignKey("Firm")]
        public Nullable<int> FirmId { get; set; }
        public Nullable<DateTime> OfferDate { get; set; }
        public string Explanation { get; set; }
        public Nullable<decimal> TotalQuantity { get; set; }
        public Nullable<decimal> TotalPrice { get; set; }

        [ForeignKey("Plant")]
        public Nullable<int> PlantId { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }

        [ForeignKey("CreatedUser")]
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }
        public string Expiration { get; set; }
        public Nullable<decimal> SheetWeight { get; set; }
        public Nullable<decimal> LaborCost { get; set; }
        public Nullable<decimal> WastageWeight { get; set; }
        public Nullable<decimal> ProfitRate { get; set; }
        public Nullable<int> CreditMonths { get; set; }
        public Nullable<int> CreditRate { get; set; }

        public virtual Firm Firm { get; set; }
        public virtual Plant Plant { get; set; }
        public virtual User CreatedUser { get; set; }

        [InverseProperty("ItemOffer")]
        public virtual ICollection<ItemOfferDetail> ItemOfferDetail { get; set; }
    }
}
