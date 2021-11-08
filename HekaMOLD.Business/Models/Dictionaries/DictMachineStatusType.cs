using HekaMOLD.Business.Models.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.Dictionaries
{
    public class DictMachineStatusType
    {
        public static Dictionary<MachineStatusType, string> Values
            = new Dictionary<MachineStatusType, string>()
        {
            { MachineStatusType.Stopped, "Bekliyor" },
            { MachineStatusType.Running, "Çalışıyor" },
        };
    }
}
