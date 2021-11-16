using HekaMOLD.Business.Base;
using HekaMOLD.Business.Models.DataTransfer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Authentication
{
    public class UserRoleModel : IDataObject
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
        public int? PlantId { get; set; }

        #region VISUAL ELEMENTS
        public UserAuthModel[] AuthTypes { get; set; }
        public UserRoleSubscriptionModel[] Subscriptions { get; set; }
        #endregion
    }
}
