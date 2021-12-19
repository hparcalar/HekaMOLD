using HekaMOLD.Business.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Production
{
   public class YarnColourGroupModel : IDataObject
    {
        public int Id { get; set; }
        public string YarnColourGroupCode { get; set; }
        public string YarnColourGroupName { get; set; }
    }
}
