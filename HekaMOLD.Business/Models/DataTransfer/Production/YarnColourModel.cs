using HekaMOLD.Business.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Production
{
   public class YarnColourModel : IDataObject
    {
        public int Id { get; set; }
        public string YarnColourCode { get; set; }
        public string YarnColourName { get; set; }
        public int YarnColourGroupId { get; set; }
        public string GroupName { get; set; }

    }
}
