using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context
{
    public class ItemOfferSheetUsage
    {
        public int Id { get; set; }

        [ForeignKey("ItemOfferDetail")]
        public Nullable<int> ItemOfferDetailId { get; set; }

        [ForeignKey("ItemOfferSheet")]
        public Nullable<int> ItemOfferSheetId { get; set; }
        public Nullable<int> Quantity { get; set; }

        public virtual ItemOfferDetail ItemOfferDetail { get; set; }
        public virtual ItemOfferSheet ItemOfferSheet { get; set; }
    }
}
