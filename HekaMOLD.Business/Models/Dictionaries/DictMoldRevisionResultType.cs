using HekaMOLD.Business.Models.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.Dictionaries
{
    public class DictMoldRevisionResultType
    {
        public static Dictionary<MoldRevisionResultType, string> Values
          = new Dictionary<MoldRevisionResultType, string>()
      {
            { MoldRevisionResultType.None, "" },
            { MoldRevisionResultType.Successful, "Başarılı" },
            { MoldRevisionResultType.Fault, "Hatalı" },
      };
    }
}
