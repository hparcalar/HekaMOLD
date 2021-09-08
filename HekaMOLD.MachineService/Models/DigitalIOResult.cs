using System;

namespace HekaMOLD.MachineService.Models
{
    public class DigitalIOResult {
        public int slot { get; set; }
        public DigitalIOArray io { get; set; }
    }
}