using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Models.Virtual
{
    public class SubscriptionCategoryModel
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public bool IsChecked { get; set; } = false;
    }
}
