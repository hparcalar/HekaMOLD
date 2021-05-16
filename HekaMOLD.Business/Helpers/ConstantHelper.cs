using HekaMOLD.Business.Models.Constants;
using HekaMOLD.Business.Models.Dictionaries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Helpers
{
    public static class ConstantHelper
    {
        public static string ToCaption(this Enum obj)
        {
            try
            {
                if (obj.GetType() == typeof(RequestStatusType))
                {
                    return DictRequestStatusType.Values
                        .Where(d => d.Key == (RequestStatusType)obj)
                        .Select(d => d.Value)
                        .FirstOrDefault();
                }
                else if (obj.GetType() == typeof(NotifyType))
                {
                    return DictNotifyType.Values
                        .Where(d => d.Key == (NotifyType)obj)
                        .Select(d => d.Value)
                        .FirstOrDefault();
                }
                else if (obj.GetType() == typeof(WarehouseType))
                {
                    return DictWarehouseType.Values
                        .Where(d => d.Key == (WarehouseType)obj)
                        .Select(d => d.Value)
                        .FirstOrDefault();
                }
                else if (obj.GetType() == typeof(OrderStatusType))
                {
                    return DictOrderStatusType.Values
                        .Where(d => d.Key == (OrderStatusType)obj)
                        .Select(d => d.Value)
                        .FirstOrDefault();
                }
            }
            catch (Exception)
            {

            }

            return string.Empty;
        }
    }
}
