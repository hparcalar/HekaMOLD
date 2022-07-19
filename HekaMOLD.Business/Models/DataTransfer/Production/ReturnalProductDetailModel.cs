using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Production
{
    public class ReturnalProductDetailModel
    {
        public int Id { get; set; }
        public Nullable<int> ReturnalProductId { get; set; }
        public Nullable<int> ItemId { get; set; }

        public Nullable<decimal> Quantity { get; set; }
        public Nullable<decimal> PackageQuantity { get; set; }
        public Nullable<decimal> PalletQuantity { get; set; }
        public string Explanation { get; set; }

        public string ItemNo { get; set; }
        public string ItemName { get; set; }
    }
}
