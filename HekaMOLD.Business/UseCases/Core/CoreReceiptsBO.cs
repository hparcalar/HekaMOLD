using Heka.DataAccess.Context;
using Heka.DataAccess.UnitOfWork;
using HekaMOLD.Business.Base;
using HekaMOLD.Business.Models.Constants;
using HekaMOLD.Business.Models.Operational;
using HekaMOLD.Business.UseCases.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.UseCases.Core
{
    public class CoreReceiptsBO : CoreSystemBO
    {
        public string GetNextOrderNo(int plantId, ItemOrderType orderType)
        {
            try
            {
                var repo = _unitOfWork.GetRepository<ItemOrder>();
                string lastReceiptNo = repo.Filter(d => d.PlantId == plantId && d.OrderType == (int)orderType)
                    .OrderByDescending(d => d.OrderNo)
                    .Select(d => d.OrderNo)
                    .FirstOrDefault();

                if (string.IsNullOrEmpty(lastReceiptNo))
                    lastReceiptNo = "0";

                return string.Format("{0:000000}", Convert.ToInt32(lastReceiptNo) + 1);
            }
            catch (Exception)
            {

            }

            return default;
        }

        public string GetNextRequestNo(int plantId)
        {
            try
            {
                var repo = _unitOfWork.GetRepository<ItemRequest>();
                string lastReceiptNo = repo.Filter(d => d.PlantId == plantId)
                    .OrderByDescending(d => d.RequestNo)
                    .Select(d => d.RequestNo)
                    .FirstOrDefault();

                if (string.IsNullOrEmpty(lastReceiptNo))
                    lastReceiptNo = "0";

                return string.Format("{0:000000}", Convert.ToInt32(lastReceiptNo) + 1);
            }
            catch (Exception)
            {

            }

            return default;
        }

        public string GetNextReceiptNo(int plantId, ItemReceiptType receiptType)
        {
            try
            {
                var repo = _unitOfWork.GetRepository<ItemReceipt>();
                string lastReceiptNo = repo.Filter(d => d.PlantId == plantId && d.ReceiptType == (int)receiptType)
                    .OrderByDescending(d => d.ReceiptNo)
                    .Select(d => d.ReceiptNo)
                    .FirstOrDefault();

                if (string.IsNullOrEmpty(lastReceiptNo))
                    lastReceiptNo = "0";

                return string.Format("{0:000000}", Convert.ToInt32(lastReceiptNo) + 1);
            }
            catch (Exception)
            {

            }

            return default;
        }
        
        public BusinessResult UpdateConsume(int? consumedId, int? consumerId, decimal usedQuantity)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<ItemReceiptConsume>();
                var existingConsume = repo.Get(d => d.ConsumedReceiptDetailId == consumedId
                    && d.ConsumerReceiptDetailId == consumerId);
                if (existingConsume == null)
                {
                    existingConsume = new ItemReceiptConsume
                    {
                        ConsumedReceiptDetailId = consumedId,
                        ConsumerReceiptDetailId = consumerId,
                        UsedQuantity = usedQuantity,
                    };
                    repo.Add(existingConsume);
                }

                existingConsume.UsedQuantity = usedQuantity;

                _unitOfWork.SaveChanges();
                result.Result = true;
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }
        public BusinessResult DeleteConsume(int? consumedId, int? consumerId)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<ItemReceiptConsume>();
                var existingConsume = repo.Get(d => d.ConsumedReceiptDetailId == consumedId
                    && d.ConsumerReceiptDetailId == consumerId);
                if (existingConsume != null)
                {
                    repo.Delete(existingConsume);
                }

                _unitOfWork.SaveChanges();
                result.Result = true;
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }
        protected BusinessResult UpdateConsume(ItemReceiptDetail consumed, ItemReceiptDetail consumer, decimal usedQuantity)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<ItemReceiptConsume>();
                var existingConsume = repo.Get(d => d.ItemReceiptDetail == consumed && d.ItemReceiptDetail1 == consumer);
                if (existingConsume == null)
                {
                    existingConsume = new ItemReceiptConsume
                    {
                        ItemReceiptDetail = consumed,
                        ItemReceiptDetail1 = consumer,
                        UsedQuantity = usedQuantity,
                    };
                    repo.Add(existingConsume);
                }

                existingConsume.UsedQuantity = usedQuantity;

                result.Result = true;
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public BusinessResult UpdateItemStats(int[] itemId)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                EFUnitOfWork _uow = new EFUnitOfWork();
                var repoItem = _uow.GetRepository<Item>();
                var repoDetail = _uow.GetRepository<ItemReceiptDetail>();
                var repoWarehouse = _uow.GetRepository<Warehouse>();
                var repoItemStatus = _uow.GetRepository<ItemLiveStatus>();

                var warehouseList = repoWarehouse.GetAll().Select(d => d.Id).ToArray();

                foreach (var item in itemId)
                {
                    foreach (var warehouseId in warehouseList)
                    {
                        var warehouseQuery = repoDetail.Filter(d => d.ItemReceipt.InWarehouseId == warehouseId
                            && d.ItemId == item);

                        var entryQty = warehouseQuery.Where(d => d.ItemReceipt.ReceiptType < 100).Sum(d => d.NetQuantity);
                        var deliverQty = warehouseQuery.Where(d => d.ItemReceipt.ReceiptType > 100).Sum(d => d.NetQuantity);

                        var dbWarehouseStats = repoItemStatus.Get(d => d.WarehouseId == warehouseId && d.ItemId == item);
                        if (dbWarehouseStats == null)
                        {
                            dbWarehouseStats = new ItemLiveStatus
                            {
                                ItemId = item,
                                WarehouseId = warehouseId
                            };
                            repoItemStatus.Add(dbWarehouseStats);
                        }

                        dbWarehouseStats.InQuantity = (entryQty ?? 0);
                        dbWarehouseStats.OutQuantity = (deliverQty ?? 0);
                        dbWarehouseStats.LiveQuantity = (entryQty ?? 0) - (deliverQty ?? 0);
                    }
                }

                _uow.SaveChanges();
                result.Result = true;
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }
    }
}
