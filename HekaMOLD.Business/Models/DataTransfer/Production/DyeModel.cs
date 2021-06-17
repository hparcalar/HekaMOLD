using HekaMOLD.Business.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Production
{
    public class DyeModel : IDataObject
    {
        public int Id { get; set; }
        public string DyeCode { get; set; }
        public string DyeName { get; set; }
        public string RalCode { get; set; }
        public bool? IsActive { get; set; }
    }
}
