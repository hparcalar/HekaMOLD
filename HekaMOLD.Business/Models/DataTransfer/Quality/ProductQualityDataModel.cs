using HekaMOLD.Business.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Quality
{
    public class ProductQualityDataModel : IDataObject
    {
        public int Id { get; set; }
        public int? MachineId { get; set; }
        public int? ProductId { get; set; }
        public DateTime? ControlDate { get; set; }
        public ProductQualityDataDetailModel[] Details { get; set; }

        #region VISUAL ELEMENTS
        public string MachineCode { get; set; }
        public string MachineName { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string ControlDateStr { get; set; }
        public string CreatedUserName { get; set; }
        #endregion
    }
}
