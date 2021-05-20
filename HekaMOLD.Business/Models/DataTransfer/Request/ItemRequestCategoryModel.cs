using HekaMOLD.Business.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Request
{
    public class ItemRequestCategoryModel : IDataObject
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
    }
}
