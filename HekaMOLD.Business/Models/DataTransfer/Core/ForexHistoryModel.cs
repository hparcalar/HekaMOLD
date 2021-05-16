using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Core
{
    public class ForexHistoryModel
    {
        public int Id { get; set; }
        public int? ForexId { get; set; }
        public DateTime? HistoryDate { get; set; }
        public decimal? SalesForexRate { get; set; }
        public decimal? BuyForexRate { get; set; }
    }
}
