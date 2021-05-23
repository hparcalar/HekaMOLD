using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Core
{
    public class RecordInformationModel
    {
        public DateTime? CreatedDate { get; set; }
        public string CreatedDateStr { get; set; }
        public int? CreatedUserId { get; set; }
        public string CreatedUserName { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedDateStr { get; set; }
        public int? UpdatedUserId { get; set; }
        public string UpdatedUserName { get; set; }

        public bool Result { get; set; }
        public string ErrorMessage { get; set; }
    }
}
