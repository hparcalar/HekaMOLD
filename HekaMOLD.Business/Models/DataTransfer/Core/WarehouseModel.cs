using HekaMOLD.Business.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Core
{
    public class WarehouseModel : IDataObject
    {
        public int Id { get; set; }
        public string WarehouseCode { get; set; }
        public string WarehouseName { get; set; }
        public int? WarehouseType { get; set; }
        public int? PlantId { get; set; }

    }
}
