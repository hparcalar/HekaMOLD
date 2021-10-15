﻿using HekaMOLD.Business.Models.Constants;
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
                else if (obj.GetType() == typeof(ItemCriticalBehaviourType))
                {
                    return DictItemCriticalBehaviourType.Values
                        .Where(d => d.Key == (ItemCriticalBehaviourType)obj)
                        .Select(d => d.Value)
                        .FirstOrDefault();
                }
                else if (obj.GetType() == typeof(ItemType))
                {
                    return DictItemType.Values
                        .Where(d => d.Key == (ItemType)obj)
                        .Select(d => d.Value)
                        .FirstOrDefault();
                }
                else if (obj.GetType() == typeof(ItemReceiptType))
                {
                    return DictItemReceiptType.Values
                        .Where(d => d.Key == (ItemReceiptType)obj)
                        .Select(d => d.Value)
                        .FirstOrDefault();
                }
                else if (obj.GetType() == typeof(ReceiptStatusType))
                {
                    return DictReceiptStatusType.Values
                        .Where(d => d.Key == (ReceiptStatusType)obj)
                        .Select(d => d.Value)
                        .FirstOrDefault();
                }
                else if (obj.GetType() == typeof(ReceiptCategoryType))
                {
                    return DictReceiptCategoryType.Values
                        .Where(d => d.Key == (ReceiptCategoryType)obj)
                        .Select(d => d.Value)
                        .FirstOrDefault();
                }
                else if (obj.GetType() == typeof(ItemOrderType))
                {
                    return DictItemOrderType.Values
                        .Where(d => d.Key == (ItemOrderType)obj)
                        .Select(d => d.Value)
                        .FirstOrDefault();
                }
                else if (obj.GetType() == typeof(WorkOrderStatusType))
                {
                    return DictWorkOrderStatusType.Values
                        .Where(d => d.Key == (WorkOrderStatusType)obj)
                        .Select(d => d.Value)
                        .FirstOrDefault();
                }
                else if (obj.GetType() == typeof(PostureStatusType))
                {
                    return DictPostureStatusType.Values
                        .Where(d => d.Key == (PostureStatusType)obj)
                        .Select(d => d.Value)
                        .FirstOrDefault();
                }
                else if (obj.GetType() == typeof(MoldStatus))
                {
                    return DictMoldStatus.Values
                        .Where(d => d.Key == (MoldStatus)obj)
                        .Select(d => d.Value)
                        .FirstOrDefault();
                }
                else if (obj.GetType() == typeof(ReportType))
                {
                    return DictReportType.Values
                        .Where(d => d.Key == (ReportType)obj)
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
