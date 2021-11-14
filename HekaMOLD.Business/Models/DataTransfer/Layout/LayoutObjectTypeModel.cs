using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Layout
{
    public class LayoutObjectTypeModel
    {
        public int Id { get; set; }
        public string ObjectTypeCode { get; set; }
        public string ObjectTypeName { get; set; }
        public byte[] ObjectData { get; set; }
        public string DataTypeExtension { get; set; }
    }
}
