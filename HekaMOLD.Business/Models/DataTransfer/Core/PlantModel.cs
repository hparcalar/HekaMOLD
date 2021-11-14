using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Core
{
    public class PlantModel
    {
        public int Id { get; set; }
        public string PlantCode { get; set; }
        public string PlantName { get; set; }
        public byte[] LogoData { get; set; }
    }
}
