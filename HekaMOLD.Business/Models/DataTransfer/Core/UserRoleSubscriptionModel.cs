using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.DataTransfer.Core
{
    public class UserRoleSubscriptionModel
    {
        public int Id { get; set; }
        public int? UserRoleId { get; set; }
        public int? SubscriptionCategory { get; set; }

        #region VISUAL ELEMENTS
        public string SubscriptionText { get; set; }
        #endregion
    }
}
