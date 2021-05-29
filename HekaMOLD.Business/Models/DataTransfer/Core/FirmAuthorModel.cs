using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Core
{
    public class FirmAuthorModel
    {
        public int Id { get; set; }
        public int? FirmId { get; set; }
        public string AuthorName { get; set; }
        public string Title { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Gsm { get; set; }
        public bool? SendMailForPurchaseOrder { get; set; }
        public bool? SendMailForProduction { get; set; }

        #region VISUAL ELEMENTS
        public bool NewDetail { get; set; }
        #endregion
    }
}
