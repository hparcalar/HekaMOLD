using HekaMOLD.Business.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Core
{
    public class ItemGroupModel : IDataObject
    {
        public int Id { get; set; }
        public string ItemGroupCode { get; set; }
        public string ItemGroupName { get; set; }
        public int? ItemCategoryId { get; set; }
        public int? PlantId { get; set; }
    }
}
