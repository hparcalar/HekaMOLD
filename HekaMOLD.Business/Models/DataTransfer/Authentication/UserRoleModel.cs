using HekaMOLD.Business.Base;
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
    }
}
