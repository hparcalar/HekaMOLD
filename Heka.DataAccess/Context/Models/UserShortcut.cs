using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heka.DataAccess.Context
{
    public partial class UserShortcut
    {
        public int Id { get; set; }

        [ForeignKey("User")]
        public Nullable<int> UserId { get; set; }
        public string Path { get; set; }
        public string Title { get; set; }
        public string Explanation { get; set; }
        public string PathParams { get; set; }
        public Nullable<int> X { get; set; }
        public Nullable<int> Y { get; set; }

        public virtual User User { get; set; }
    }
}
