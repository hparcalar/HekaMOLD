using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Production
{
    public class ReturnalProductModel
    {
        public int Id { get; set; }
        public string ReturnCode { get; set; }
        public Nullable<DateTime> ReturnDate { get; set; }
        public string Explanation { get; set; }
        public Nullable<int> PlantId { get; set; }
    }
}
