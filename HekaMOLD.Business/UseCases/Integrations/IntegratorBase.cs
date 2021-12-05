using HekaMOLD.Business.Models.DataTransfer.Core;
using HekaMOLD.Business.Models.DataTransfer.Production;
using HekaMOLD.Business.Models.DataTransfer.Receipt;
using HekaMOLD.Business.Models.Operational;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.UseCases.Integrations
{
    public interface IntegratorBase
    {
        event EventHandler OnTransferError;
        BusinessResult PullItems(SyncPointModel syncPoint);
        BusinessResult PullFirms(SyncPointModel syncPoint);
        BusinessResult PullRecipes(SyncPointModel syncPoint);
        BusinessResult PullUnits(SyncPointModel syncPoint);
        BusinessResult PushPurchasingWaybills(SyncPointModel syncPoint, ItemReceiptModel[] receipts);
        BusinessResult PullSaleOrders(SyncPointModel syncPoint);
        BusinessResult PushFinishedProducts(SyncPointModel syncPoint, WorkOrderModel[] workOrders);
        BusinessResult PushDeliveryReceipts(SyncPointModel syncPoint, ItemReceiptModel[] receipts);
    }
}
