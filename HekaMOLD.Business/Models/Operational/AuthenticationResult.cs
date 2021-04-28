using HekaMOLD.Business.Models.DataTransfer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.Authentication
{
    public class AuthenticationResult
    {
        public bool Result { get; set; }
        public string ErrorMessage { get; set; }
        public UserModel UserData { get; set; }
    }
}
