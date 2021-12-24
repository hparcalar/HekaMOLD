using HekaMOLD.Business.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Warehouse
{
    public class PalletModel : IDataObject
    {
        public int Id { get; set; }
        public string PalletNo { get; set; }
        public int PalletStatus { get; set; }
        public Nullable<int> PlantId { get; set; }
    }
}
