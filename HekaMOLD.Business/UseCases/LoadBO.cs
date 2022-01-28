using Heka.DataAccess.Context;
using Heka.DataAccess.Context.Models;
using HekaMOLD.Business.Helpers;
using HekaMOLD.Business.Models.Constants;
using HekaMOLD.Business.Models.DataTransfer.Logistics;
using HekaMOLD.Business.Models.Operational;
using HekaMOLD.Business.UseCases.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HekaMOLD.Business.UseCases
{
    public class LoadBO : CoreReceiptsBO
    {

        public string GetNextLoadCode(int plantId, int directionId = 0)
        {
            string defaultValue = "";
            try
            {

                var repo = _unitOfWork.GetRepository<ItemLoad>();
                int lastOrderNo = repo.GetAll()
                    .OrderByDescending(d => d.Id)
                    .Select(d => d.Id)
                    .FirstOrDefault();

                if (string.IsNullOrEmpty(Convert.ToString(lastOrderNo)))
                    lastOrderNo = 0;
                defaultValue = DateTime.Now.Year + "-" + string.Format("{0:000000}", Convert.ToInt32(lastOrderNo) + 1);
                return  defaultValue+ ((OrderTransactionDirectionType)directionId).ToCaption();
            }
            catch (Exception)
            {

            }

            return defaultValue;
        }
        public ItemLoadModel[] GetItemLoadList()
        {
            List<ItemLoadModel> data = new List<ItemLoadModel>();

            var repo = _unitOfWork.GetRepository<ItemLoad>();

            repo.GetAll().ToList().ForEach(d =>
            {
                ItemLoadModel containerObj = new ItemLoadModel();
                d.MapTo(containerObj); 
                containerObj.LoadStatusTypeStr = ((LoadStatusType)d.LoadStatusType.Value).ToCaption();
                containerObj.DischargeDateStr = string.Format("{0:dd.MM.yyyy}", d.DischargeDate);
                containerObj.LoadingDateStr = string.Format("{0:dd.MM.yyyy}", d.LoadingDate);
                containerObj.DateOfNeedStr = string.Format("{0:dd.MM.yyyy}", d.DateOfNeed );
                containerObj.LoadOutDateStr = string.Format("{0:dd.MM.yyyy}", d.LoadOutDate);
                containerObj.CustomerFirmName = d.FirmCustomer != null ? d.FirmCustomer.FirmName : "";

                data.Add(containerObj);
            });

            return data.ToArray();
        }

        public ItemLoadModel GetLoad(int id)
        {
            ItemLoadModel model = new ItemLoadModel { Details = new ItemLoadDetailModel[0] };

            var repo = _unitOfWork.GetRepository<ItemLoad>();
            var repoDetails = _unitOfWork.GetRepository<ItemLoadDetail>();

            var dbObj = repo.Get(d => d.Id == id);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);
                model.DateOfNeedStr = string.Format("{0:dd.MM.yyyy}", dbObj.DateOfNeed );
                model.OrderDateStr = string.Format("{0:dd.MM.yyyy}", dbObj.ItemOrder != null ? dbObj.ItemOrder.OrderDate : System.DateTime.Today);
                model.DischargeDateStr = string.Format("{0:dd.MM.yyyy}", dbObj.DischargeDate);
                model.LoadingDateStr = string.Format("{0:dd.MM.yyyy}", dbObj.LoadingDate);
                model.LoadOutDateStr = string.Format("{0:dd.MM.yyyy}",  dbObj.LoadOutDate );
                model.LoadDateStr = string.Format("{0:dd.MM.yyyy}", dbObj.LoadOutDate);
                model.OrderNo = dbObj.OrderNo;
                model.CustomerFirmCode = dbObj.FirmCustomer != null ? dbObj.FirmCustomer.FirmCode : "";
                model.CustomerFirmName = dbObj.FirmCustomer != null ? dbObj.FirmCustomer.FirmName : "";
                model.EntryCustomsCode = dbObj.CustomsEntry != null ? dbObj.CustomsEntry.CustomsCode : "";
                model.EntryCustomsName = dbObj.CustomsEntry != null ? dbObj.CustomsEntry.CustomsName : "";
                model.ExitCustomsCode = dbObj.CustomsExit != null ? dbObj.CustomsExit.CustomsCode : "";
                model.ExitCustomsName = dbObj.CustomsExit != null ? dbObj.CustomsExit.CustomsName : "";
                //model.OrderCreatUser = dbObj.ItemOrder.User != null ? dbObj.ItemOrder.User.Login:"";
                model.OrderTransactionDirectionTypeStr = dbObj.OrderTransactionDirectionType == 1 ? LSabit.GET_ABROAD_EXPORT : dbObj.OrderTransactionDirectionType == 2 ? LSabit.GET_ABROAD_IMPORT :
                    dbObj.OrderTransactionDirectionType == 3 ? LSabit.GET_DOMESTIC : dbObj.OrderTransactionDirectionType == 4 ? LSabit.GET_DOMESTIC_TRASFER : dbObj.OrderTransactionDirectionType == 5 ? LSabit.GET_ABROAD_TRASFER : "";
                model.OrderUploadTypeStr = dbObj.OrderUploadType == 1 ? LSabit.GET_GRUPAJ : dbObj.OrderUploadType == 2 ? LSabit.GET_COMPLATE : "";
                model.OrderUploadPointTypeStr =  dbObj.OrderUploadPointType == 1 ? LSabit.GET_FROMCUSTOMER : dbObj.OrderUploadPointType == 2 ? LSabit.GET_FROMWAREHOUSE : "" ;
                model.OrderCalculationTypeStr =  dbObj.OrderCalculationType == 1 ? LSabit.GET_WEIGHTTED : dbObj.OrderCalculationType == 2 ? LSabit.GET_VOLUMETRIC : dbObj.OrderCalculationType == 3 ? LSabit.GET_LADAMETRE : "";
                model.Details =
                    repoDetails.Filter(d => d.ItemLoadId == dbObj.Id)
                    .Select(d => new ItemLoadDetailModel
                    {
                        Id = d.Id,
                        ItemId = d.ItemId,
                        CreatedDate = d.CreatedDate,
                        CreatedUserId = d.CreatedUserId,
                        ItemLoadId = d.ItemLoadId,
                        ItemOrderDetailId = d.ItemOrderDetailId,
                        LineNumber = d.LineNumber,
                        NetQuantity = d.NetQuantity,
                        NewDetail = false,
                        LoadStatus = d.LoadStatus,
                        Quantity = d.Quantity,
                        ShortWidth = d.ShortWidth,
                        LongWidth = d.LongWidth,
                        Height = d.Height,
                        Weight = d.Weight,
                        Volume = d.Volume,
                        Stackable = d.Stackable,
                        PackageInNumber = d.PackageInNumber,
                        UnitId = d.UnitId,
                        UnitPrice = d.UnitPrice,
                        UpdatedDate = d.UpdatedDate,
                        UpdatedUserId = d.UpdatedUserId,
                        ItemCode = d.Item != null ? d.Item.ItemNo : "",
                        ItemName = d.Item != null ? d.Item.ItemName : "",

                    }).ToArray();
            }

            return model;
        }

        public BusinessResult DeleteLoad(int id)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<ItemLoad>();
                var repoDetail = _unitOfWork.GetRepository<ItemLoadDetail>();
                var repoNotify = _unitOfWork.GetRepository<Notification>();

                var dbObj = repo.Get(d => d.Id == id);
                if (dbObj == null)
                    throw new Exception("Silinmesi istenen yük kaydına ulaşılamadı.");

                //TODO: Sefere Dönüştürülmüş Yük silinemez
                //if (dbObj.ItemReceipt.Any())
                //    throw new Exception("İrsaliyesi girilmiş olan bir sipariş silinemez.");

                // CLEAR DETAILS
                if (dbObj.ItemLoadDetail.Any())
                {
                    var detailObjArr = dbObj.ItemLoadDetail.ToArray();
                    foreach (var item in detailObjArr)
                    {
                        #region SET REQUEST & DETAIL TO APPROVED
                        if (item.ItemOrderDetail != null)
                        {
                            item.ItemOrderDetail.OrderStatus = (int)OrderStatusType.Approved;
                            item.ItemOrderDetail.ItemOrder.OrderStatus = (int)RequestStatusType.Approved;
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

        public BusinessResult SaveOrUpdateLoad(ItemLoadModel model, bool detailCanBeNull = false)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<ItemLoad>();
                var repoDetail = _unitOfWork.GetRepository<ItemLoadDetail>();
                var repoOrderDetail = _unitOfWork.GetRepository<ItemOrderDetail>();
                var repoNotify = _unitOfWork.GetRepository<Notification>();

                bool newRecord = false;
                var dbObj = repo.Get(d => d.Id == model.Id);
                if (dbObj == null)
                {
                    dbObj = new ItemLoad();
                    dbObj.OrderNo = "Yapılandır"; //GetNextOrderNo(model.PlantId.Value, (ItemOrderType)model.OrderType.Value);
                    dbObj.CreatDate = DateTime.Now;
                    dbObj.CreatedUserId = model.CreatedUserId;
                    dbObj.LoadStatusType = (int)LoadStatusType.Created;
                    repo.Add(dbObj);
                    newRecord = true;
                }

                if (!string.IsNullOrEmpty(model.LoadDateStr))
                {
                    model.LoadDate = DateTime.ParseExact(model.LoadDateStr, "dd.MM.yyyy",
                        System.Globalization.CultureInfo.GetCultureInfo("tr"));
                }
                if (!string.IsNullOrEmpty(model.DischargeDateStr))
                {
                    model.DischargeDate = DateTime.ParseExact(model.DischargeDateStr, "dd.MM.yyyy",
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

                var crDate = dbObj.CreatDate;
                var reqStats = dbObj.LoadStatusType;
                var crUserId = dbObj.CreatedUserId;

                model.MapTo(dbObj);

                if (dbObj.CreatDate == null)
                    dbObj.CreatDate = crDate;
                if (dbObj.LoadStatusType == null)
                    dbObj.LoadStatusType = reqStats;
                if (dbObj.CreatedUserId == null)
                    dbObj.CreatedUserId = crUserId;

                dbObj.UpdatedDate = DateTime.Now;

                #region SAVE DETAILS
                if (model.Details == null && detailCanBeNull == false)
                    throw new Exception("Detay bilgisi olmadan yük kaydedilemez.");

                foreach (var item in model.Details)
                {
                    if (item.NewDetail)
                        item.Id = 0;
                }

                if (dbObj.LoadStatusType != (int)OrderStatusType.Completed &&  dbObj.LoadStatusType != (int)OrderStatusType.Cancelled)
                {
                    var newDetailIdList = model.Details.Select(d => d.Id).ToArray();
                    var deletedDetails = dbObj.ItemLoadDetail.Where(d => !newDetailIdList.Contains(d.Id)).ToArray();
                    foreach (var item in deletedDetails)
                    {
                        //if (item.ItemReceiptDetail.Any())
                        //    continue;
                        ////throw new Exception("İrsaliyesi girilmiş olan bir sipariş detayı silinemez.");

                        //if (item.WorkOrderDetail.Any())
                        //    continue;

                        #region SET ORDER & DETAIL TO APPROVED
                        if (item.ItemOrderDetail != null)
                        {
                            item.ItemOrderDetail.OrderStatus = (int)OrderStatusType.Approved;
                            item.ItemOrderDetail.ItemOrder.OrderStatus = (int)OrderStatusType.Approved;
                        }
                        #endregion

                        repoDetail.Delete(item);
                    }

                    int lineNo = 1;
                    foreach (var item in model.Details)
                    {
                        var dbDetail = repoDetail.Get(d => d.Id == item.Id);
                        if (dbDetail == null)
                        {
                            dbDetail = new ItemLoadDetail
                            {
                                ItemLoad = dbObj,
                                LoadStatus = dbObj.LoadStatusType
                            };

                            repoDetail.Add(dbDetail);
                        }

                        item.MapTo(dbDetail);
                        dbDetail.ItemLoad = dbObj;

                        if (dbDetail.LoadStatus == null || dbDetail.LoadStatus == (int)LoadStatusType.Approved)
                            dbDetail.LoadStatus = dbObj.LoadStatusType;
                        if (dbObj.Id > 0)
                            dbDetail.ItemLoadId = dbObj.Id;

                        dbDetail.LineNumber = lineNo;

                        #region SET REQUEST & DETAIL STATUS TO COMPLETE
                        if (dbDetail.ItemOrderDetailId > 0)
                        {
                            var dbOrderDetail = repoOrderDetail.Get(d => d.Id == dbDetail.ItemOrderDetailId);
                            if (dbOrderDetail != null)
                            {
                                dbOrderDetail.OrderStatus = (int)OrderStatusType.Completed;

                                if (!dbOrderDetail.ItemOrder
                                    .ItemOrderDetail.Any(d => d.OrderStatus != (int)OrderStatusType.Completed))
                                {
                                    dbOrderDetail.ItemOrder.OrderStatus = (int)OrderStatusType.Completed;
                                }
                            }
                        }
                        #endregion

                        lineNo++;
                    }
                }
                #endregion

                _unitOfWork.SaveChanges();

                #region CREATE NOTIFICATION
                    if (newRecord || !repoNotify.Any(d => d.RecordId == dbObj.Id && d.NotifyType == (int)NotifyType.ItemLoadWaitForApproval))
                    {
                        var repoUser = _unitOfWork.GetRepository<User>();
                        var itemLoadApprovalOwners = repoUser.Filter(d => d.UserRole != null &&
                            d.UserRole.UserAuth.Any(m => m.UserAuthType.AuthTypeCode == "LoadApproval" && m.IsGranted == true)).ToArray();

                        foreach (var poOWNER in itemLoadApprovalOwners)
                        {
                            base.CreateNotification(new Models.DataTransfer.Core.NotificationModel
                            {
                                IsProcessed = false,
                                Message = //string.Format("{0:dd.MM.yyyy}", dbObj.OrderDate)+ 
                                "Yeni bir yük oluşturuldu. Onayınız bekleniyor.",
                                Title = NotifyType.ItemLoadWaitForApproval.ToCaption(),
                                NotifyType = (int)NotifyType.ItemLoadWaitForApproval,
                                SeenStatus = 0,
                                RecordId = dbObj.Id,
                                UserId = poOWNER.Id
                            });
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



    }
}
