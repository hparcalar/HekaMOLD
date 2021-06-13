using HekaMOLD.Business.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Production
{
    public class ProductRecipeModel : IDataObject
    {
        public int Id { get; set; }
        public string ProductRecipeCode { get; set; }
        public string Description { get; set; }
        public int? ProductRecipeType { get; set; }
        public int? ProductId { get; set; }
        public bool? IsActive { get; set; }
        public ProductRecipeDetailModel[] Details { get; set; }

        #region VISUAL ELEMENTS
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string CreatedDateStr { get; set; }
        #endregion
    }
}
