using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heka.DataAccess.Context
{
    public class ReturnalProduct
    {
        public ReturnalProduct()
        {
            this.ReturnalProductDetail = new HashSet<ReturnalProductDetail>();
        }

        public int Id { get; set; }
        public string ReturnCode { get; set; }
        public Nullable<DateTime> ReturnDate { get; set; }
        public string Explanation { get; set; }

        [ForeignKey("Plant")]
        public Nullable<int> PlantId { get; set; }

        public virtual Plant Plant { get; set; }

        [InverseProperty("ReturnalProduct")]
        public virtual ICollection<ReturnalProductDetail> ReturnalProductDetail { get; set; }
    }
}
