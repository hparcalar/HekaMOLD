using HekaMOLD.Business.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Core
{
    public class UserModel : IDataObject
    {
        public int Id { get; set; }
        public string UserCode { get; set; }
        public string UserName { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public int? PlantId { get; set; }
        public int? UserRoleId { get; set; }

        #region VISUAL ELEMENTS
        public string RoleName { get; set; }
        #endregion
    }
}
