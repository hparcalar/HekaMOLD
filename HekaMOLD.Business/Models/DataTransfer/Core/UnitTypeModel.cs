using HekaMOLD.Business.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Core
{
    public class UnitTypeModel : IDataObject
    {
        public int Id { get; set; }
        public string UnitCode { get; set; }
        public string UnitName { get; set; }
        public int? PlantId { get; set; }
    }
}
