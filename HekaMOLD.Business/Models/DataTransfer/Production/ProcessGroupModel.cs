using HekaMOLD.Business.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Production
{
    public class ProcessGroupModel : IDataObject
    {
        public int Id { get; set; }
        public string ProcessGroupCode { get; set; }
        public string ProcessGroupName { get; set; }
        public int? PlantId { get; set; }
    }
}
