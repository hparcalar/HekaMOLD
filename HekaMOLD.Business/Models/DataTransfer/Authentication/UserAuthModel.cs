using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Authentication
{
    public class UserAuthModel
    {
        public int Id { get; set; }
        public int? AuthTypeId { get; set; }
        public int? UserRoleId { get; set; }
        public int? UserId { get; set; }
        public bool? IsGranted { get; set; }

        #region VISUAL ELEMENTS
        public string AuthTypeCode { get; set; }
        #endregion
    }
}
