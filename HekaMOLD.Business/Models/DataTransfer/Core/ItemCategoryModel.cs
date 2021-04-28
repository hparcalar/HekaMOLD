using HekaMOLD.Business.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Core
{
    public class ItemCategoryModel : IDataObject
    {
        public int Id { get; set; }
        public string ItemCategoryCode { get; set; }
        public string ItemCategoryName { get; set; }
        public int? PlantId { get; set; }
    }
}
