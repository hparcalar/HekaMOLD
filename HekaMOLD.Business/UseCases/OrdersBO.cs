using Heka.DataAccess.Context;
using Heka.DataAccess.Context.Models;
using HekaMOLD.Business.Helpers;
using HekaMOLD.Business.Models.Constants;
using HekaMOLD.Business.Models.DataTransfer.Order;
using HekaMOLD.Business.Models.Operational;
using HekaMOLD.Business.UseCases.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace HekaMOLD.Business.UseCases
{
    public class OrdersBO : CoreReceiptsBO
    {
        public ItemOrderModel[] GetItemOrderList(ItemOrderType orderType)
        {
            List<ItemOrderModel> data = new List<ItemOrderModel>();
            var repo = _unitOfWork.GetRepository<ItemOrder>();

            return repo.Filter(d => d.OrderType == (int)orderType).ToList()
                .Select(d => new ItemOrderModel
                {
                    Id = d.Id,
                    OrderStatusStr = ((OrderStatusType)d.OrderStatus.Value).ToCaption(),
                    OrderDateStr = string.Format("{0:dd.MM.yyyy}", d.OrderDate),
                    DateOfNeedStr = string.Format("{0:dd.MM.yyyy}", d.DateOfNeed),
                    LoadOutDateStr = string.Format("{0:dd.MM.yyyy}", d.LoadOutDate),
                    ScheduledUploadDateStr = string.Format("{0:dd.MM.yyyy}", d.ScheduledUploadDate),
                    OrderDateWeek = getYearAndWeekOfNumber(Convert.ToString( d.OrderDate )),
                    CustomerFirmCode = d.Firm != null ? d.Firm.FirmCode : "",
                    CustomerFirmName = d.Firm != null ? d.Firm.FirmName : "",
                    LoadCityName = d.LoadCity != null ? d.LoadCity.CityName : "",
                    LoadPostCode = d.LoadCity != null ? d.LoadCity.PostCode : "",
                    EntryCustomsName = d.CustomsEntry != null ? d.CustomsEntry.CustomsName : "",
                    ExitCustomsName = d.CustomsExit != null ? d.CustomsExit.CustomsName : "", 
                    DischangeCityName = d.DischargeCity != null ? d.DischargeCity.CityName : "",
                    DischangePostCode = d.DischargeCity != null ? d.DischargeCity.PostCode :"",
                    OrderTransactionDirectionTypeStr = d.OrderTransactionDirectionType !=null ? ((OrderTransactionDirectionType)d.OrderTransactionDirectionType).ToCaption() :"",
                    OrderUploadTypeStr = d.OrderUploadType == 1 ? LSabit.GET_GRUPAJ : d.OrderUploadType == 2 ? LSabit.GET_COMPLATE : "",
                    OrderUploadPointTypeStr = d.OrderUploadPointType == 1 ? LSabit.GET_FROMCUSTOMER : d.OrderUploadPointType == 2 ? LSabit.GET_FROMWAREHOUSE : "",
                    OrderCalculationTypeStr = d.OrderCalculationType == 1 ? LSabit.GET_WEIGHTTED : d.OrderCalculationType == 2 ? LSabit.GET_VOLUMETRIC : d.OrderCalculationType == 3 ? LSabit.GET_LADAMETRE : d.OrderCalculationType == 4 ? LSabit.GET_COMPLET : d.OrderCalculationType == 5 ? LSabit.GET_MINIMUM : "",
                    CreatedUserName = d.User != null ? d.User.UserName : "",
                    //OrderDateWeek = Convert.ToString( Convert.ToDateTime(d.OrderDate).GetDateTimeFormats()),
                    //LoadCountryName = d.LoadCity.Country != null ? d.LoadCity.Country.CountryName :"",
                    OveralQuantity = d.OveralQuantity,
                    OveralWeight = d.OveralWeight,
                    OveralVolume = d.OveralVolume,
                    OveralLadametre = d.OveralLadametre,
                    OverallTotal = d.OverallTotal,
                    OrderNo = d.OrderNo,
                    OrderDate = d.OrderDate,
                    DocumentNo = d.DocumentNo,
                    Explanation = d.Explanation,
                    CustomerFirmId = d.CustomerFirmId,
                    OrderStatus = d.OrderStatus,
                    OrderType = d.OrderType,
                    CalculationTypePrice = d.CalculationTypePrice,
                    PlantId = d.PlantId,
                    ForexTypeCode = d.ForexType != null ? d.ForexType.ForexTypeCode :"",
                })
                .OrderByDescending(d => d.OrderDate)
                .ToArray();
        }
        public ItemOrderModel[] GetUnappovedItemOrderList(ItemOrderType orderType)
        {
            List<ItemOrderModel> data = new List<ItemOrderModel>();

            var repo = _unitOfWork.GetRepository<ItemOrder>();

            return repo.Filter(d => d.OrderType == (int)orderType && d.OrderStatus == (int)OrderStatusType.Created ).ToList()
                .Select(d => new ItemOrderModel
                {
                    Id = d.Id,
                    OrderStatusStr = ((OrderStatusType)d.OrderStatus.Value).ToCaption(),
                    CreatedDateStr = string.Format("{0:dd.MM.yyyy}", d.CreatedDate),
                    DateOfNeedStr = string.Format("{0:dd.MM.yyyy}", d.DateOfNeed),
                    LoadOutDateStr = string.Format("{0:dd.MM.yyyy}", d.LoadOutDate),
                    ScheduledUploadDateStr = string.Format("{0:dd.MM.yyyy}", d.ScheduledUploadDate),
                    CustomerFirmCode = d.Firm != null ? d.Firm.FirmCode : "",
                    CustomerFirmName = d.Firm != null ? d.Firm.FirmName : "",
                    LoadCityName = d.LoadCity != null ? d.LoadCity.CityName : "",
                    LoadPostCode = d.LoadCity != null ? d.LoadCity.PostCode : "",
                    EntryCustomsName = d.CustomsEntry != null ? d.CustomsEntry.CustomsName : "",
                    ExitCustomsName = d.CustomsExit != null ? d.CustomsExit.CustomsName : "",
                    DischangeCityName = d.DischargeCity != null ? d.DischargeCity.CityName : "",
                    DischangePostCode = d.DischargeCity != null ? d.DischargeCity.PostCode : "",
                    OrderTransactionDirectionTypeStr = d.OrderTransactionDirectionType != null ? ((OrderTransactionDirectionType)d.OrderTransactionDirectionType).ToCaption() : "",
                    OrderUploadTypeStr = d.OrderUploadType == 1 ? LSabit.GET_GRUPAJ : d.OrderUploadType == 2 ? LSabit.GET_COMPLATE : "",
                    OrderUploadPointTypeStr = d.OrderUploadPointType == 1 ? LSabit.GET_FROMCUSTOMER : d.OrderUploadPointType == 2 ? LSabit.GET_FROMWAREHOUSE : "",
                    OrderCalculationTypeStr = d.OrderCalculationType == 1 ? LSabit.GET_WEIGHTTED : d.OrderCalculationType == 2 ? LSabit.GET_VOLUMETRIC : d.OrderCalculationType == 3 ? LSabit.GET_LADAMETRE : d.OrderCalculationType == 4 ? LSabit.GET_COMPLET : d.OrderCalculationType == 5 ? LSabit.GET_MINIMUM : "",
                    // LoadCountryName = d.LoadCity.Country != null ? d.LoadCity.Country.CountryName :"",
                    OveralQuantity = d.OveralQuantity,
                    OveralWeight = d.OveralWeight,
                    OveralVolume = d.OveralVolume,
                    OveralLadametre = d.OveralLadametre,
                    OverallTotal = d.OverallTotal,
                    OrderNo = d.OrderNo,
                    OrderDate = d.OrderDate,
                    DocumentNo = d.DocumentNo,
                    Explanation = d.Explanation,
                    CustomerFirmId = d.CustomerFirmId,
                    OrderStatus = d.OrderStatus,
                    OrderType = d.OrderType,
                    CalculationTypePrice = d.CalculationTypePrice,
                    PlantId = d.PlantId,
                    ForexTypeCode = d.ForexType != null ? d.ForexType.ForexTypeCode : ""
                })
                .OrderByDescending(d => d.OrderDate)
                .ToArray();
        }
        public BusinessResult SaveOrUpdateItemOrder(ItemOrderModel model,int userId,bool detailCanBeNull = false)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<ItemOrder>();
                var repoDetail = _unitOfWork.GetRepository<ItemOrderDetail>();
                var repoRequestDetail = _unitOfWork.GetRepository<ItemRequestDetail>();
                var repoNotify = _unitOfWork.GetRepository<Notification>();

                bool newRecord = false;
                var dbObj = repo.Get(d => d.Id == model.Id);
                if (dbObj == null)
                {
                    dbObj = new ItemOrder();
                    dbObj.OrderNo = GetNextOrderNo(model.PlantId.Value, (ItemOrderType)model.OrderType.Value);
                    dbObj.CreatedDate = DateTime.Now;
                    dbObj.CreatedUserId = userId;
                    dbObj.OrderStatus = (int)OrderStatusType.Created;
                    repo.Add(dbObj);
                    newRecord = true;
                }
                if (!string.IsNullOrEmpty(model.OrderDateStr))
                {
                    model.OrderDate = DateTime.ParseExact(model.OrderDateStr, "dd.MM.yyyy",
                        System.Globalization.CultureInfo.GetCultureInfo("tr"));
                }

                if (!string.IsNullOrEmpty(model.DateOfNeedStr))
                {
                    model.DateOfNeed = DateTime.ParseExact(model.DateOfNeedStr, "dd.MM.yyyy",
                        System.Globalization.CultureInfo.GetCultureInfo("tr"));
                }
                if (!string.IsNullOrEmpty(model.LoadOutDateStr))
                {
                    model.LoadOutDate = DateTime.ParseExact(model.LoadOutDateStr, "dd.MM.yyyy",
                        System.Globalization.CultureInfo.GetCultureInfo("tr"));
                }
                if (!string.IsNullOrEmpty(model.ScheduledUploadDateStr))
                {
                    model.ScheduledUploadDate = DateTime.ParseExact(model.ScheduledUploadDateStr, "dd.MM.yyyy",
                        System.Globalization.CultureInfo.GetCultureInfo("tr"));
                }
                else if (string.IsNullOrEmpty(model.ScheduledUploadDateStr))
                    throw new Exception("Planlanan yükleme tarihi giriniz !");

                if ((int)model.OrderStatus == (int)OrderStatusType.Loaded)
                {
                    throw new Exception("Yüke dönüştürülülen siparişte değişiklik yapılamaz !");
                }
                if ( model.OrderTransactionDirectionType == null)
                    throw new Exception("İşlem yönü seçilmelidir !");

                if (model.ForexTypeId == null)
                    throw new Exception("Döviz kodu seçilmelidir !");

                var crDate = dbObj.CreatedDate;
                var donDate = dbObj.DateOfNeed;
                var reqStats = dbObj.OrderStatus;
                var crUserId = dbObj.CreatedUserId;

                model.MapTo(dbObj);
                dbObj.UpdatedUserId = userId;
                if (dbObj.CreatedDate == null)
                    dbObj.CreatedDate = crDate;
                if (dbObj.DateOfNeed == null)
                    dbObj.DateOfNeed = donDate;
                if (dbObj.OrderStatus == null)
                    dbObj.OrderStatus = reqStats;
                if (dbObj.CreatedUserId == null)
                    dbObj.CreatedUserId = crUserId;

                dbObj.UpdatedDate = DateTime.Now;
                #region SAVE DETAILS
                if (model.Details == null && detailCanBeNull == false)
                    throw new Exception("Detay bilgisi olmadan sipariş kaydedilemez.");

                foreach (var item in model.Details)
                {
                    if (item.NewDetail)
                        item.Id = 0;
                }

                if (dbObj.OrderStatus != (int)OrderStatusType.Completed &&
                    dbObj.OrderStatus != (int)OrderStatusType.Cancelled)
                {
                    var newDetailIdList = model.Details.Select(d => d.Id).ToArray();
                    var deletedDetails = dbObj.ItemOrderDetail.Where(d => !newDetailIdList.Contains(d.Id)).ToArray();
                    foreach (var item in deletedDetails)
                    {
                        //if (item.ItemReceiptDetail.Any())
                        //    continue;
                        ////throw new Exception("İrsaliyesi girilmiş olan bir sipariş detayı silinemez.");

                        //if (item.WorkOrderDetail.Any())
                        //    continue;

                        #region SET REQUEST & DETAIL TO APPROVED
                        //if (item.ItemRequestDetail != null)
                        //{
                        //    item.ItemRequestDetail.RequestStatus = (int)RequestStatusType.Approved;
                        //    item.ItemRequestDetail.ItemRequest.RequestStatus = (int)RequestStatusType.Approved;
                        //}
                        #endregion

                        repoDetail.Delete(item);
                    }

                    int lineNo = 1;
                    foreach (var item in model.Details)
                    {
                        var dbDetail = repoDetail.Get(d => d.Id == item.Id);
                        if (dbDetail == null)
                        {
                            dbDetail = new ItemOrderDetail
                            {
                                ItemOrder = dbObj,
                                OrderStatus = dbObj.OrderStatus
                            };

                            repoDetail.Add(dbDetail);
                        }

                        item.MapTo(dbDetail);
                        dbDetail.ItemOrder = dbObj;

                        if (dbDetail.OrderStatus == null || dbDetail.OrderStatus == (int)OrderStatusType.Approved)
                            dbDetail.OrderStatus = dbObj.OrderStatus;
                        if (dbObj.Id > 0)
                            dbDetail.ItemOrderId = dbObj.Id;

                        dbDetail.LineNumber = lineNo;

                        #region SET REQUEST & DETAIL STATUS TO COMPLETE
                        //if (dbDetail.ItemRequestDetailId > 0)
                        //{
                        //    var dbRequestDetail = repoRequestDetail.Get(d => d.Id == dbDetail.ItemRequestDetailId);
                        //    if (dbRequestDetail != null)
                        //    {
                        //        dbRequestDetail.RequestStatus = (int)RequestStatusType.Completed;

                        //        if (!dbRequestDetail.ItemRequest
                        //            .ItemRequestDetail.Any(d => d.RequestStatus != (int)RequestStatusType.Completed))
                        //        {
                        //            dbRequestDetail.ItemRequest.RequestStatus = (int)RequestStatusType.Completed;
                        //        }
                        //    }
                        //}
                        #endregion

                        lineNo++;
                    }
                }
                #endregion

                _unitOfWork.SaveChanges();

                #region CREATE NOTIFICATION
                if (model.OrderType == (int)ItemOrderType.Sale)
                {
                    if (newRecord || !repoNotify.Any(d => d.RecordId == dbObj.Id && d.NotifyType == (int)NotifyType.ItemOrderWaitForApproval))
                    {
                        var repoUser = _unitOfWork.GetRepository<User>();
                        var itemOrderApprovalOwners = repoUser.Filter(d => d.UserRole != null &&
                            d.UserRole.UserAuth.Any(m => m.UserAuthType.AuthTypeCode == "LOApproval" && m.IsGranted == true)).ToArray();

                        foreach (var poOWNER in itemOrderApprovalOwners)
                        {
                            base.CreateNotification(new Models.DataTransfer.Core.NotificationModel
                            {
                                IsProcessed = false,
                                Message = "Sipariş Kodu: "+dbObj.OrderNo 
                                + " yeni bir sipariş oluşturuldu. Onayınız bekleniyor.",
                                Title = NotifyType.ItemOrderWaitForApproval.ToCaption(),
                                NotifyType = (int)NotifyType.ItemOrderWaitForApproval,
                                SeenStatus = 0,
                                RecordId = dbObj.Id,
                                UserId = poOWNER.Id
                            });
                        }
                    }

                }
                #endregion

                result.Result = true;
                result.RecordId = dbObj.Id;
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public BusinessResult AddOrderDetail(int orderId, ItemOrderDetailModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repoOrder = _unitOfWork.GetRepository<ItemOrder>();
                var repoOrderDetail = _unitOfWork.GetRepository<ItemOrderDetail>();

                var dbOrder = repoOrder.Get(d => d.Id == orderId);
                if (dbOrder == null)
                    throw new Exception("Sipariş bilgisi HEKA yazılımında bulunamadı.");

                var dbNewDetail = new ItemOrderDetail();
                model.MapTo(dbNewDetail);
                dbNewDetail.ItemOrder = dbOrder;
                repoOrderDetail.Add(dbNewDetail);

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

        public BusinessResult UpdateOrderDetail(ItemOrderDetailModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repoOrderDetail = _unitOfWork.GetRepository<ItemOrderDetail>();

                var dbOrder = repoOrderDetail.Get(d => d.Id == model.Id);
                if (dbOrder == null)
                    throw new Exception("Sipariş kalem bilgisi HEKA yazılımında bulunamadı.");

                dbOrder.Quantity = model.Quantity;
                dbOrder.UnitPrice = model.UnitPrice;
                dbOrder.SubTotal = model.SubTotal;
                dbOrder.TaxAmount = model.TaxAmount;
                dbOrder.ForexRate = model.ForexRate;
                dbOrder.ForexId = model.ForexId;

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
        public BusinessResult DeleteItemOrder(int id)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<ItemOrder>();
                var repoDetail = _unitOfWork.GetRepository<ItemOrderDetail>();
                var repoNotify = _unitOfWork.GetRepository<Notification>();
                var repoLoad = _unitOfWork.GetRepository<ItemLoad>();


                var dbObj = repo.Get(d => d.Id == id);
                if (dbObj == null)
                    throw new Exception("Silinmesi istenen sipariş kaydına ulaşılamadı.");

                var dbLoad = repoLoad.Get(d => d.OrderNo == dbObj.OrderNo);
                if (dbLoad!=null)
                    throw new Exception("Yüke dönüştürülmüş sipariş silinemez ! Yük No: "+dbLoad.LoadCode);

                if (dbObj.ItemReceipt.Any())
                    throw new Exception("İrsaliyesi girilmiş olan bir sipariş silinemez.");

                // CLEAR DETAILS
                if (dbObj.ItemOrderDetail.Any())
                {
                    var detailObjArr = dbObj.ItemOrderDetail.ToArray();
                    foreach (var item in detailObjArr)
                    {
                        #region SET REQUEST & DETAIL TO APPROVED
                        if (item.ItemRequestDetail != null)
                        {
                            item.ItemRequestDetail.RequestStatus = (int)RequestStatusType.Approved;
                            item.ItemRequestDetail.ItemRequest.RequestStatus = (int)RequestStatusType.Approved;
                        }
                        #endregion

                        repoDetail.Delete(item);
                    }
                }

                //// CLEAR NEEDS
                //if (dbObj.ItemOrderItemNeeds.Any())
                //{
                //    var needs = dbObj.ItemOrderItemNeeds.ToArray();
                //    foreach (var needItem in needs)
                //    {
                //        repoNeeds.Delete(needItem);
                //    }
                //}

                // CLEAR NOTIFICATIONS
                if (repoNotify.Any(d => d.NotifyType == (int)NotifyType.ItemOrderWaitForApproval && d.RecordId == dbObj.Id))
                {
                    var notificationArr = repoNotify.Filter(d => d.NotifyType == (int)NotifyType.ItemOrderWaitForApproval && d.RecordId == dbObj.Id)
                        .ToArray();

                    foreach (var item in notificationArr)
                    {
                        repoNotify.Delete(item);
                    }
                }

                repo.Delete(dbObj);
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

        public ItemOrderModel GetItemOrder(int id)
        {
            ItemOrderModel model = new ItemOrderModel { Details = new ItemOrderDetailModel[0] };

            var repo = _unitOfWork.GetRepository<ItemOrder>();
            var repoDetails = _unitOfWork.GetRepository<ItemOrderDetail>();

            var dbObj = repo.Get(d => d.Id == id);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);
                model.DateOfNeedStr = string.Format("{0:dd.MM.yyyy}", dbObj.DateOfNeed);
                model.OrderDateStr = string.Format("{0:dd.MM.yyyy}", dbObj.OrderDate);
                model.LoadOutDateStr = string.Format("{0:dd.MM.yyyy}", dbObj.LoadOutDate);
                model.ScheduledUploadDateStr = string.Format("{0:dd.MM.yyyy}", dbObj.ScheduledUploadDate);
                model.OrderStatusStr = ((OrderStatusType)model.OrderStatus).ToCaption();
                model.CustomerFirmCode = dbObj.Firm != null ? dbObj.Firm.FirmCode : "";
                model.CustomerFirmName = dbObj.Firm != null ? dbObj.Firm.FirmName : "";
                model.CreatedUserName = dbObj.User != null ? dbObj.User.UserName : "";

                model.Details =
                    repoDetails.Filter(d => d.ItemOrderId == dbObj.Id)
                    .Select(d => new ItemOrderDetailModel
                    {
                        Id = d.Id,
                        ItemId = d.ItemId,
                        CreatedDate = d.CreatedDate,
                        CreatedUserId = d.CreatedUserId,
                        Explanation = d.Explanation,
                        ForexId = d.ForexId,
                        ForexRate = d.ForexRate,
                        ForexUnitPrice = d.ForexUnitPrice,
                        GrossQuantity = d.GrossQuantity,
                        ItemOrderId = d.ItemOrderId,
                        ItemRequestDetailId = d.ItemRequestDetailId,
                        LineNumber = d.LineNumber,
                        NetQuantity = d.NetQuantity,
                        NewDetail = false,
                        OrderStatus = d.OrderStatus,
                        OverallTotal = d.OverallTotal,
                        Quantity = d.Quantity,
                        ShortWidth = d.ShortWidth,
                        LongWidth = d.LongWidth,
                        Height = d.Height,
                        Weight = d.Weight,
                        Volume = d.Volume,
                        Ladametre = d.Ladametre,
                        Stackable = d.Stackable,
                        PackageInNumber = d.PackageInNumber,
                        SubTotal = d.SubTotal,
                        TaxAmount = d.TaxAmount,
                        TaxIncluded = d.TaxIncluded,
                        TaxRate = d.TaxRate,
                        UnitId = d.UnitId,
                        UnitPrice = d.UnitPrice,
                        UpdatedDate = d.UpdatedDate,
                        UpdatedUserId = d.UpdatedUserId,
                        ItemNo = d.Item != null ? d.Item.ItemNo : "",
                        ItemName = d.Item != null ? d.Item.ItemName : "",
                        UnitCode = d.UnitType != null ? d.UnitType.UnitCode : "",
                        UnitName = d.UnitType != null ? d.UnitType.UnitName : "",
                    }).ToArray();
            }

            return model;
        }

        public ItemOrderModel GetItemOrder(string documentNo, ItemOrderType orderType)
        {
            ItemOrderModel model = new ItemOrderModel { Details = new ItemOrderDetailModel[0] };

            var repo = _unitOfWork.GetRepository<ItemOrder>();
            var dbObj = repo.Get(d => d.DocumentNo == documentNo && d.OrderType == (int)orderType);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);
                model.DateOfNeedStr = string.Format("{0:dd.MM.yyyy}", dbObj.DateOfNeed);
                model.OrderDateStr = string.Format("{0:dd.MM.yyyy}", dbObj.OrderDate);
                model.ScheduledUploadDateStr = string.Format("{0:dd.MM.yyyy}", dbObj.ScheduledUploadDate);
                model.OrderStatusStr = ((OrderStatusType)model.OrderStatus).ToCaption();
                model.CustomerFirmCode = dbObj.Firm != null ? dbObj.Firm.FirmCode : "";
                model.CustomerFirmName = dbObj.Firm != null ? dbObj.Firm.FirmName : "";
                model.WarehouseCode = dbObj.Warehouse != null ? dbObj.Warehouse.WarehouseCode : "";
                model.WarehouseName = dbObj.Warehouse != null ? dbObj.Warehouse.WarehouseName : "";

                List<ItemOrderDetailModel> detailContainers = new List<ItemOrderDetailModel>();
                dbObj.ItemOrderDetail.ToList().ForEach(d =>
                {
                    ItemOrderDetailModel detailContainerObj = new ItemOrderDetailModel();
                    d.MapTo(detailContainerObj);
                    detailContainerObj.ItemNo = d.Item != null ? d.Item.ItemNo : "";
                    detailContainerObj.ItemName = d.Item != null ? d.Item.ItemName : "";
                    detailContainerObj.UnitCode = d.UnitType != null ? d.UnitType.UnitCode : "";
                    detailContainerObj.UnitName = d.UnitType != null ? d.UnitType.UnitName : "";
                    detailContainerObj.NewDetail = false;
                    detailContainers.Add(detailContainerObj);
                });

                model.Details = detailContainers.ToArray();
            }

            return model;
        }

        public ItemOrderDetailModel CalculateOrderDetail(ItemOrderDetailModel model)
        {
            if (model.ForexId > 0 && model.ForexRate > 0)
            {
                model.ForexUnitPrice = model.UnitPrice / model.ForexRate;
            }

            decimal? overallTotal = 0;
            decimal? taxExtractedUnitPrice = 0;

            if (model.TaxIncluded == true)
            {
                decimal? taxIncludedUnitPrice = (model.UnitPrice / (1 + (model.TaxRate / 100m)));
                overallTotal = taxIncludedUnitPrice * model.Quantity;
                taxExtractedUnitPrice = taxIncludedUnitPrice;
            }
            else
            {
                overallTotal = model.UnitPrice * model.Quantity;
                taxExtractedUnitPrice = model.UnitPrice;
            }

            model.OverallTotal = overallTotal;
            model.TaxAmount = overallTotal * model.TaxRate / 100.0m;

            return model;
        }

        public BusinessResult ApproveItemOrderPrice(int id, int userId)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<ItemOrder>();

                var dbObj = repo.Get(d => d.Id == id);
                if (dbObj == null)
                    throw new Exception("Onaylanması beklenen sipariş kaydına ulaşılamadı.");

                if (dbObj.OrderStatus != (int)OrderStatusType.Created)
                    throw new Exception("Onay bekleyen durumunda olmayan bir sipariş onaylanamaz.");

                dbObj.OrderStatus = (int)OrderStatusType.Approved;
                dbObj.UpdatedDate = DateTime.Now;
                dbObj.UpdatedUserId = userId;

                foreach (var item in dbObj.ItemOrderDetail)
                {
                    item.OrderStatus = (int)OrderStatusType.Approved;
                }

                _unitOfWork.SaveChanges();

                #region CREATE NOTIFICATIONS
                base.CreateNotification(new Models.DataTransfer.Core.NotificationModel
                {
                    IsProcessed = false,
                    Message = string.Format("{0:dd.MM.yyyy}", dbObj.OrderDate)
                            + " tarihinde oluşturduğunuz siparişi onaylandı.",
                    Title = NotifyType.ItemOrderIsApproved.ToCaption(),
                    NotifyType = (int)NotifyType.ItemOrderIsApproved,
                    SeenStatus = 0,
                    RecordId = dbObj.Id,
                    UserId = dbObj.CreatedUserId
                });
                #endregion

                result.RecordId = dbObj.Id;
                result.Result = true;
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public BusinessResult CancelledItemOrderPrice(int id, int userId)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<ItemOrder>();

                var dbObj = repo.Get(d => d.Id == id);
                if (dbObj == null)
                    throw new Exception("Onaylanması beklenen sipariş kaydına ulaşılamadı.");

                if (dbObj.OrderStatus == (int)OrderStatusType.Cancelled)
                    throw new Exception("Bu sipariş daha önceden iptal edilmiştir.");

                if (dbObj.OrderStatus == (int)OrderStatusType.Approved)
                    throw new Exception("Onaylı siparişi iptal etmek için onayını kaldırmanız gerekmektedir.");

                dbObj.OrderStatus = (int)OrderStatusType.Cancelled;
                dbObj.UpdatedDate = DateTime.Now;
                dbObj.UpdatedUserId = userId;

                foreach (var item in dbObj.ItemOrderDetail)
                {
                    item.OrderStatus = (int)OrderStatusType.Cancelled;
                }

                _unitOfWork.SaveChanges();

                #region CREATE NOTIFICATIONS
                base.CreateNotification(new Models.DataTransfer.Core.NotificationModel
                {
                    IsProcessed = false,
                    Message = string.Format("{0:dd.MM.yyyy}", dbObj.DateOfNeed)
                            + " tarihinde oluşturduğunuz satınalma siparişi iptal edildi.",
                    //Title = NotifyType.ItemOrderIsApproved.ToCaption(),
                    //NotifyType = (int)NotifyType.ItemOrderIsApproved,
                    SeenStatus = 0,
                    RecordId = dbObj.Id,
                    UserId = dbObj.CreatedUserId
                });
                #endregion

                result.RecordId = dbObj.Id;
                result.Result = true;
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }


        public BusinessResult ToggleOrderDetailStatus(int orderDetailId)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repoDetail = _unitOfWork.GetRepository<ItemOrderDetail>();

                var dbDetail = repoDetail.Get(d => d.Id == orderDetailId);
                if (dbDetail == null)
                    throw new Exception("İşlem yapılması istenen sipariş kalemi kaydı artık bulunamadı.");

                var dbHeader = dbDetail.ItemOrder;

                if (dbDetail.OrderStatus == (int)OrderStatusType.Completed)
                    dbDetail.OrderStatus = (int)OrderStatusType.Approved;
                else if (dbDetail.OrderStatus != (int)OrderStatusType.Completed)
                    dbDetail.OrderStatus = (int)OrderStatusType.Completed;

                if (!dbHeader.ItemOrderDetail.Any(d => d.OrderStatus != (int)OrderStatusType.Completed))
                    dbHeader.OrderStatus = (int)OrderStatusType.Completed;
                else
                {
                    if (dbHeader.ItemOrderDetail.Any(d => d.WorkOrderDetail.Any()))
                        dbHeader.OrderStatus = (int)OrderStatusType.Planned;
                    else
                        dbHeader.OrderStatus = (int)OrderStatusType.Approved;
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

        public bool HasAnySaleOrder(string documentNo)
        {
            var repo = _unitOfWork.GetRepository<ItemOrder>();
            return repo.Any(d => d.DocumentNo == documentNo && d.OrderType == (int)ItemOrderType.Sale);
        }

        public bool HasAnySaleOrderBySyncKey(string syncKey)
        {
            var repo = _unitOfWork.GetRepository<ItemOrder>();
            return repo.Any(d => d.SyncKey == syncKey);
        }

        #region CONSUMINGS
        public BusinessResult UpdateOrderConsume(int? orderDetailId, int? consumedId, int? consumerId, decimal usedQuantity)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<ItemOrderConsume>();
                var existingConsume = repo.Get(d =>
                    (d.ConsumedReceiptDetailId == consumedId || d.ConsumerReceiptDetailId == consumerId)
                    && d.ItemOrderDetailId == orderDetailId);
                if (existingConsume == null)
                {
                    existingConsume = new ItemOrderConsume
                    {
                        ConsumedReceiptDetailId = consumedId,
                        ConsumerReceiptDetailId = consumerId,
                        ItemOrderDetailId = orderDetailId,
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
        public BusinessResult DeleteOrderConsume(int? orderDetailId, int? consumedId, int? consumerId)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<ItemOrderConsume>();
                var existingConsume = repo.Get(d =>
                    (d.ConsumedReceiptDetailId == consumedId || d.ConsumerReceiptDetailId == consumerId)
                    && d.ItemOrderDetailId == orderDetailId);
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
        #endregion

        #region ORDER PRESENTATION
        public ItemOrderModel[] GetRelatedOrders(int receiptId)
        {
            ItemOrderModel[] data = new ItemOrderModel[0];

            try
            {
                var repo = _unitOfWork.GetRepository<ItemReceipt>();
                var repoUser = _unitOfWork.GetRepository<User>();

                var dbObj = repo.GetById(receiptId);
                if (dbObj == null)
                    throw new Exception("İrsaliye bilgisine ulaşılamadı.");

                List<ItemOrder> relatedOrders = new List<ItemOrder>();

                foreach (var item in dbObj.ItemReceiptDetail)
                {
                    if (item.ItemOrderDetail != null
                        && !relatedOrders.Any(d => d.Id == item.ItemOrderDetail.ItemOrderId))
                        relatedOrders.Add(item.ItemOrderDetail.ItemOrder);
                }

                List<ItemOrderModel> sumData = new List<ItemOrderModel>();
                relatedOrders.ForEach(d =>
                {
                    ItemOrderModel model = new ItemOrderModel();
                    d.MapTo(model);

                    if (d.CreatedUserId != null)
                    {
                        var dbUser = repoUser.GetById(d.CreatedUserId.Value);
                        if (dbUser != null)
                        {
                            model.CreatedUserName = dbUser.UserName;
                        }
                    }

                    model.CustomerFirmName = d.Firm != null ? d.Firm.FirmName : "";
                    model.CustomerFirmCode = d.Firm != null ? d.Firm.FirmCode : "";
                    model.CreatedDateStr = string.Format("{0:dd.MM.yyyy HH:mm}", d.CreatedDate);
                    model.OrderDateStr = string.Format("{0:dd.MM.yyyy HH:mm}", d.OrderDate);
                    sumData.Add(model);
                });

                data = sumData.ToArray();
            }
            catch (Exception)
            {

            }

            return data;
        }

        public ItemOrderDetailModel[] GetApprovedPurchaseOrderDetails(int plantId)
        {
            ItemOrderDetailModel[] data = new ItemOrderDetailModel[0];

            try
            {
                var repo = _unitOfWork.GetRepository<ItemOrderDetail>();

                data = repo.Filter(d => d.ItemOrder.PlantId == plantId &&
                    d.ItemOrder.OrderType == (int)ItemOrderType.Purchase &&
                    d.OrderStatus == (int)OrderStatusType.Approved
                    && d.ItemOrder.OrderStatus == (int)OrderStatusType.Approved)
                    .ToList()
                    .Select(d => new ItemOrderDetailModel
                    {
                        Id = d.Id,
                        Quantity = d.Quantity,
                        FirmId = d.ItemOrder.CustomerFirmId,
                        FirmCode = d.ItemOrder.Firm != null ?
                            d.ItemOrder.Firm.FirmCode : "",
                        FirmName = d.ItemOrder.Firm != null ?
                            d.ItemOrder.Firm.FirmName : "",
                        OrderDate = d.ItemOrder.OrderDate,
                        OrderDateStr = string.Format("{0:dd.MM.yyyy}", d.ItemOrder.OrderDate),
                        CreatedDate = d.ItemOrder.CreatedDate,
                        Explanation = d.Explanation,
                        OrderExplanation = d.ItemOrder.Explanation,
                        ItemNo = d.Item.ItemNo,
                        ItemName = d.Item.ItemName,
                        ItemId = d.ItemId,
                        OrderNo = d.ItemOrder.OrderNo,
                        LineNumber = d.LineNumber,
                        UnitId = d.UnitId,
                        UnitCode = d.UnitType != null ? d.UnitType.UnitCode : "",
                        UnitName = d.UnitType != null ? d.UnitType.UnitName : ""
                    }).ToArray();
            }
            catch (Exception)
            {

            }

            return data;
        }

        public ItemOrderDetailModel[] GetOpenSaleOrderDetails(int plantId)
        {
            ItemOrderDetailModel[] data = new ItemOrderDetailModel[0];

            try
            {
                var repo = _unitOfWork.GetRepository<ItemOrderDetail>();

                data = repo.Filter(d => d.ItemOrder.PlantId == plantId
                    && d.ItemOrder.OrderType == (int)ItemOrderType.Sale
                    && d.OrderStatus != (int)OrderStatusType.Cancelled
                    && d.OrderStatus != (int)OrderStatusType.Completed)
                    .ToList()
                    .Select(d => new ItemOrderDetailModel
                    {
                        Id = d.Id,
                        Quantity = d.Quantity,
                        FirmId = d.ItemOrder.CustomerFirmId,
                        FirmCode = d.ItemOrder.Firm != null ?
                            d.ItemOrder.Firm.FirmCode : "",
                        FirmName = d.ItemOrder.Firm != null ?
                            d.ItemOrder.Firm.FirmName : "",
                        OrderDate = d.ItemOrder.OrderDate,
                        OrderDateStr = string.Format("{0:dd.MM.yyyy}", d.ItemOrder.OrderDate),
                        CreatedDate = d.ItemOrder.CreatedDate,
                        Explanation = d.Explanation,
                        OrderExplanation = d.ItemOrder.Explanation,
                        ItemNo = d.Item.ItemNo,
                        ItemName = d.Item.ItemName,
                        ItemId = d.ItemId,
                        OrderNo = d.ItemOrder.OrderNo,
                        LineNumber = d.LineNumber,
                        UnitId = d.UnitId,
                        UnitCode = d.UnitType != null ? d.UnitType.UnitCode : "",
                        UnitName = d.UnitType != null ? d.UnitType.UnitName : ""
                    }).ToArray();
            }
            catch (Exception)
            {

            }

            return data;
        }
        #endregion

        #region STATUS VALIDATIONS
        public BusinessResult CheckOrderDetailStatus(int orderDetailId)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<ItemOrderDetail>();
                var repoWorkDetail = _unitOfWork.GetRepository<WorkOrderDetail>();

                var dbDetail = repo.Get(d => d.Id == orderDetailId);
                if (dbDetail == null)
                    throw new Exception("Sipariş kalemi bulunamadı.");



                result.Result = false;
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }
        #endregion
        #region METHOD
        public string getYearAndWeekOfNumber(string OrderDateWeek)
        {
            try
            {
                DateTime Date;
                if (!string.IsNullOrEmpty(OrderDateWeek))
                     Date = Convert.ToDateTime(OrderDateWeek);
                else
                 return "";
                DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(Date);
                if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
                {
                    Date = Date.AddDays(3);
                }
                return Date.Year + "-" + CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(Date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            }
            catch (Exception)
            {

                throw;
            }

        }
        #endregion

    }
}
