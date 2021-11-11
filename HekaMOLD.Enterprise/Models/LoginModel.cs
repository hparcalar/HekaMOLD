using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HekaMOLD.Enterprise.Models
{
    public class LoginModel
    {
        public int UserId { get; set; }
        public int MachineId { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }
}