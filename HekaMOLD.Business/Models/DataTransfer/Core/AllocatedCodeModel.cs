using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Core
{
    public class AllocatedCodeModel
    {
        public int Id { get; set; }
        public string AllocatedCode { get; set; }
        public int? ObjectType { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
