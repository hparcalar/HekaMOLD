using HekaMOLD.Business.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Files
{
    public class AttachmentModel : IDataObject
    {
        public int Id { get; set; }
        public int? RecordId { get; set; }
        public int? RecordType { get; set; }
        public string Description { get; set; }
        public byte[] BinaryContent { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }

        #region VISUAL COMPONENTS
        public string CreatedDateStr { get; set; }
        #endregion
    }
}
