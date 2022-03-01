using Heka.DataAccess.Context;
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
    public class VoyageBO : CoreReceiptsBO
    {
        public string GetNextVoyageCode(int directionId = 0)
        {
            string defaultValue = "";
            try
            {
                var repo = _unitOfWork.GetRepository<CodeCounter>();
                var dbCodeCounter = repo.Filter(d => d.CounterType == 3)// CounterType type=1(Order) type=2(Load) type=3(Voyage)
                    .OrderByDescending(d => d.Id)
                    .Select(d => d)
                    .FirstOrDefault();
                defaultValue = dbCodeCounter.FirstValue + string.Format("{0:00000}", Convert.ToInt32(directionId == 1 ? (int)dbCodeCounter.Export : directionId == 2 ? (int)dbCodeCounter.Import : directionId == 3 ? (int)dbCodeCounter.Domestic : directionId == 4 ?
                    (int)dbCodeCounter.Transit : dbCodeCounter.Id) + 1) + ((OrderTransactionDirectionType)directionId).ToCaption();
                return defaultValue;
            }
            catch (Exception)
            {

            }

            return defaultValue;
        }
        public BusinessResult SaveOrUpdateVoyage(VoyageModel model, int userId, bool detailCanBeNull = false)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repoVoyage = _unitOfWork.GetRepository<Voyage>();
                var repoVoyageDetail = _unitOfWork.GetRepository<VoyageDetail>();
                var repoLoad = _unitOfWork.GetRepository<ItemLoad>();
                var repoLoadDetail = _unitOfWork.GetRepository<ItemLoadDetail>();
                var repoNotify = _unitOfWork.GetRepository<Notification>();
                var repoCodeCounter = _unitOfWork.GetRepository<CodeCounter>();

                if (model.OrderTransactionDirectionType == null)
                    throw new Exception("İşlem yönü seçilmelidir !");

                if (model.DriverId == null)
                    throw new Exception("Şoför seçilmelidir !");

                if (model.TraillerVehicleId == null)
                    throw new Exception("Romörk seçilmelidir !");

                bool newRecord = false;
                var dbObj = repoVoyage.Get(d => d.Id == model.Id);
                if (dbObj == null)
                {
                    dbObj = new Voyage();
                    dbObj.VoyageCode = GetNextVoyageCode((int)model.OrderTransactionDirectionType);
                    dbObj.CreatedDate = DateTime.Now;
                    dbObj.CreatedUserId = userId;
                    dbObj.VoyageStatus = (int)VoyageStatus.Created;
                    repoVoyage.Add(dbObj);
                    newRecord = true;
                }

                if (!string.IsNullOrEmpty(model.VoyageDateStr))
                {
                    model.VoyageDate = DateTime.ParseExact(model.VoyageDateStr, "dd.MM.yyyy",
                        System.Globalization.CultureInfo.GetCultureInfo("tr"));
                }

                if (!string.IsNullOrEmpty(model.ClosedDateStr))
                {
                    model.ClosedDate = DateTime.ParseExact(model.ClosedDateStr, "dd.MM.yyyy",
                        System.Globalization.CultureInfo.GetCultureInfo("tr"));
                }
                if (!string.IsNullOrEmpty(model.CustomsDoorEntryDateStr))
                {
                    model.CustomsDoorEntryDate = DateTime.ParseExact(model.CustomsDoorEntryDateStr, "dd.MM.yyyy",
                        System.Globalization.CultureInfo.GetCultureInfo("tr"));
                }
                if (!string.IsNullOrEmpty(model.CustomsDoorExitDateStr))
                {
                    model.CustomsDoorExitDate = DateTime.ParseExact(model.CustomsDoorExitDateStr, "dd.MM.yyyy",
                        System.Globalization.CultureInfo.GetCultureInfo("tr"));
                }
                if (!string.IsNullOrEmpty(model.StartDateStr))
                {
                    model.StartDate = DateTime.ParseExact(model.StartDateStr, "dd.MM.yyyy",
                        System.Globalization.CultureInfo.GetCultureInfo("tr"));
                }
                if (!string.IsNullOrEmpty(model.EndDateStr))
                {
                    model.EndDate = DateTime.ParseExact(model.EndDateStr, "dd.MM.yyyy",
                        System.Globalization.CultureInfo.GetCultureInfo("tr"));
                }
                if (!string.IsNullOrEmpty(model.LoadDateStr))
                {
                    model.LoadDate = DateTime.ParseExact(model.LoadDateStr, "dd.MM.yyyy",
                        System.Globalization.CultureInfo.GetCultureInfo("tr"));
                }
                if (!string.IsNullOrEmpty(model.TraillerRationCardClosedDateStr))
                {
                    model.TraillerRationCardClosedDate = DateTime.ParseExact(model.TraillerRationCardClosedDateStr, "dd.MM.yyyy",
                        System.Globalization.CultureInfo.GetCultureInfo("tr"));
                }
                if (!string.IsNullOrEmpty(model.VehicleExitDateStr))
                {
                    model.VehicleExitDate = DateTime.ParseExact(model.VehicleExitDateStr, "dd.MM.yyyy",
                        System.Globalization.CultureInfo.GetCultureInfo("tr"));
                }

                else if (string.IsNullOrEmpty(model.VoyageDateStr))
                    throw new Exception("Sefer tarihi bilgisini giriniz !");

                if (dbObj.VoyageStatus == (int)VoyageStatus.Cancelled)
                    throw new Exception("İptal edilen Sefer değişiklik yapılamaz !");


                var crDate = dbObj.CreatedDate;
                var reqStats = dbObj.VoyageStatus;
                var crUserId = dbObj.CreatedUserId;

                model.MapTo(dbObj);

                if (dbObj.CreatedDate == null)
                    dbObj.CreatedDate = crDate;
                if (dbObj.VoyageStatus == null)
                    dbObj.VoyageStatus = reqStats;
                if (dbObj.CreatedUserId == null)
                    dbObj.CreatedUserId = crUserId;
                dbObj.UpdatedDate = DateTime.Now;

                #region SAVE DETAILS
                if (model.VoyageDetails == null && detailCanBeNull == false)
                    throw new Exception("Detay bilgisi olmadan sefer kaydedilemez.");

                foreach (var item in model.VoyageDetails)
                {
                    if (item.NewDetail)
                        item.Id = 0;
                }
                if (newRecord && (dbObj.VoyageStatus != (int)VoyageStatus.Created || dbObj.VoyageStatus != (int)VoyageStatus.Approved))
                {

                    foreach (var x in model.VoyageDetails)
                    {
                        var dbItemLoad = repoLoad.Get(d => d.Id == x.ItemLoadId);

                        dbItemLoad.LoadStatusType = (int)LoadStatusType.ConvertedToVoyage;
                        dbItemLoad.VoyageCode = model.VoyageCode;
                        dbItemLoad.VoyageCreatedUserId = model.CreatedUserId;
                        dbItemLoad.VehicleTraillerId = model.TraillerVehicleId;
                        //x.MapTo(dbItemLoad);
                    }

                }

                if (dbObj.VoyageStatus != (int)LoadStatusType.Completed && dbObj.VoyageStatus != (int)LoadStatusType.Cancelled)
                {
                    var newDetailIdList = model.VoyageDetails.Select(d => d.Id).ToArray();
                    var deletedDetails = dbObj.VoyageDetail.Where(d => !newDetailIdList.Contains(d.Id)).ToArray();
                    foreach (var item in deletedDetails)
                    {
                        //if (item.ItemReceiptDetail.Any())
                        //    continue;
                        ////throw new Exception("İrsaliyesi girilmiş olan bir sipariş detayı silinemez.");

                        //if (item.WorkOrderDetail.Any())
                        //    continue;

                        #region SET ORDER & DETAIL TO APPROVED
                        if (item.ItemLoad != null)
                        {
                            item.ItemLoad.LoadStatusType = (int)LoadStatusType.Ready;
                            foreach (var itemTmp in item.ItemLoad.ItemLoadDetail)
                            {
                                itemTmp.LoadStatus = (int)LoadStatusType.Ready;
                            }
                        }
                        #endregion

                        repoVoyageDetail.Delete(item);
                    }

                    int lineNo = 1;
                    foreach (var item in model.VoyageDetails)
                    {
                        var dbVoyageDetail = repoVoyageDetail.Get(d => d.ItemLoadId == item.ItemLoadId);
                        if (dbVoyageDetail == null)
                        {
                            dbVoyageDetail = new VoyageDetail
                            {
                                Voyage = dbObj,
                                VoyageStatus = dbObj.VoyageStatus,

                            };

                            repoVoyageDetail.Add(dbVoyageDetail);
                        }

                        item.MapTo(dbVoyageDetail);
                        dbVoyageDetail.Voyage = dbObj;

                        //if (dbVoyageDetail.VoyageStatus == null || dbVoyageDetail.VoyageStatus == (int)LoadStatusType.Ready)
                        dbVoyageDetail.VoyageStatus = dbObj.VoyageStatus;
                        if (dbObj.Id > 0)
                            dbVoyageDetail.VoyageId = dbObj.Id;

                        dbVoyageDetail.LineNumber = lineNo;

                        #region SET REQUEST & DETAIL STATUS TO COMPLETE
                        //if (dbDetail.ItemOrderDetailId > 0)
                        //{
                        //    var dbOrderDetail = repoOrderDetail.Get(d => d.Id == dbDetail.ItemOrderDetailId);
                        //    if (dbOrderDetail != null)
                        //    {
                        //        dbOrderDetail.OrderStatus = (int)OrderStatusType.Completed;

                        //        if (!dbOrderDetail.ItemOrder
                        //            .ItemOrderDetail.Any(d => d.OrderStatus != (int)OrderStatusType.Completed))
                        //        {
                        //            dbOrderDetail.ItemOrder.OrderStatus = (int)OrderStatusType.Completed;
                        //        }
                        //    }
                        //}
                        #endregion

                        lineNo++;
                    }
                }
                #endregion

                #region CODECOUNTER
                var objCodeCounter = repoCodeCounter.Filter(d => d.CounterType == 3)
                    .OrderByDescending(d => d.Id)
                    .Select(d => d)
                    .FirstOrDefault();
                var dbrepoCodeCounter = repoCodeCounter.Get(d => d.Id == objCodeCounter.Id);
                if (newRecord)
                {
                    if (model.OrderTransactionDirectionType == 1)
                    {
                        dbrepoCodeCounter.Export++;
                    }
                    if (model.OrderTransactionDirectionType == 2)
                    {
                        dbrepoCodeCounter.Import++;
                    }
                    if (model.OrderTransactionDirectionType == 3)
                    {
                        dbrepoCodeCounter.Domestic++;
                    }
                    if (model.OrderTransactionDirectionType == 4)
                    {
                        dbrepoCodeCounter.Transit++;
                    }
                }
                #endregion
                _unitOfWork.SaveChanges();
                #region CREATE NOTIFICATION
                if (newRecord || !repoNotify.Any(d => d.RecordId == dbObj.Id && d.NotifyType == (int)NotifyType.VoyageWaitForApproval))
                {
                    var repoUser = _unitOfWork.GetRepository<User>();
                    var voyageApprovalOwners = repoUser.Filter(d => d.UserRole != null &&
                        d.UserRole.UserAuth.Any(m => m.UserAuthType.AuthTypeCode == "VoyageApproval" && m.IsGranted == true)).ToArray();

                    foreach (var poOWNER in voyageApprovalOwners)
                    {
                        base.CreateNotification(new Models.DataTransfer.Core.NotificationModel
                        {
                            IsProcessed = false,
                            Message = //string.Format("{0:dd.MM.yyyy}", dbObj.OrderDate)+ 
                            "Yeni bir sefer oluşturuldu. Onayınız bekleniyor.",
                            Title = NotifyType.VoyageWaitForApproval.ToCaption(),
                            NotifyType = (int)NotifyType.VoyageWaitForApproval,
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
        public VoyageModel GetVoyage(int id)
        {
            VoyageModel model = new VoyageModel { VoyageDetails = new VoyageDetailModel[0] };

            var repo = _unitOfWork.GetRepository<Voyage>();
            var repoDetails = _unitOfWork.GetRepository<VoyageDetail>();

            var dbObj = repo.Get(d => d.Id == id);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);
                model.VoyageDateStr = string.Format("{0:dd.MM.yyyy}", dbObj.VoyageDate);
                model.CustomsDoorEntryDateStr = string.Format("{0:dd.MM.yyyy}", dbObj.CustomsDoorEntryDate);
                model.CustomsDoorExitDateStr = string.Format("{0:dd.MM.yyyy}", dbObj.CustomsDoorExitDate);
                model.EndDateStr = string.Format("{0:dd.MM.yyyy}", dbObj.EndDate);
                model.VoyageStatusStr = ((VoyageStatus)model.VoyageStatus).ToCaption();
                model.LoadDateStr = string.Format("{0:dd.MM.yyyy}", dbObj.LoadDate);
                model.StartDateStr = string.Format("{0:dd.MM.yyyy}", dbObj.StartDate);
                model.TraillerRationCardClosedDateStr = string.Format("{0:dd.MM.yyyy}", dbObj.TraillerRationCardClosedDate);
                model.ClosedDateStr = string.Format("{0:dd.MM.yyyy}", dbObj.ClosedDate);
                model.VehicleExitDateStr = string.Format("{0:dd.MM.yyyy}", dbObj.VehicleExitDate);

                //model.TraillerTypeStr = 
                model.OrderTransactionDirectionTypeStr = dbObj.OrderTransactionDirectionType != null ? ((OrderTransactionDirectionType)dbObj.OrderTransactionDirectionType).ToCaption() : "";


                model.VoyageDetails =
                    repoDetails.Filter(d => d.VoyageId == dbObj.Id)
                    .Select(d => new VoyageDetailModel
                    {
                        Id = d.Id,
                        DischargeLineNo = d.DischargeLineNo,
                        ItemLoadId = d.ItemLoadId,
                        LoadCode = d.LoadCode,
                        //LoadingDateStr = string.Format("{0:dd.MM.yyyy}", d.LoadingDate),
                        //LoadOutDateStr = string.Format("{0:dd.MM.yyyy}", d.LoadOutDate),
                        //OrderTransactionDirectionTypeStr = d.OrderTransactionDirectionType != null ? ((OrderTransactionDirectionType)dbObj.OrderTransactionDirectionType).ToCaption() : "",
                        OveralQuantity = d.OveralQuantity,
                        OveralWeight = d.OveralWeight,
                        OveralLadametre = d.OveralLadametre,
                        OveralVolume = d.OveralVolume,
                        OverallTotal = d.OverallTotal,
                        OrderNo = d.OrderNo,
                        //LoadDateStr = string.Format("{0:dd.MM.yyyy}", d.LoadDate),
                        //DischargeDateStr = string.Format("{0:dd.MM.yyyy}", d.DischargeDate),
                        CalculationTypePrice = d.CalculationTypePrice,
                        DocumentNo = d.DocumentNo,
                        OrderUploadType = d.OrderUploadType,
                        OrderUploadPointType = d.OrderUploadPointType,
                        //ScheduledUploadDateStr = string.Format("{0:dd.MM.yyyy}", d.ScheduledUploadDate),
                        //DateOfNeedStr = string.Format("{0:dd.MM.yyyy}", d.DateOfNeed),
                        InvoiceId = d.InvoiceId,
                        ForexTypeId = d.ForexTypeId,
                        InvoiceStatus = d.InvoiceStatus,
                        InvoiceFreightPrice = d.InvoiceFreightPrice,
                        CmrNo = d.CmrNo,
                        CmrStatus = d.CmrStatus,
                        ShipperFirmExplanation = d.ShipperFirmExplanation,
                        BuyerFirmExplanation = d.BuyerFirmExplanation,
                        //ReadinessDateStr = string.Format("{0:dd.MM.yyyy}", d.ReadinessDate),
                        //DeliveryFromCustomerDateStr = string.Format("{0:dd.MM.yyyy}", d.DeliveryFromCustomerDate),
                        //IntendedArrivalDateStr = string.Format("{0:dd.MM.yyyy}", d.IntendedArrivalDate),
                        FirmCustomsArrivalId = d.FirmCustomsArrivalId,
                        CustomsExplanation = d.CustomsExplanation,
                        T1T2No = d.T1T2No,
                        //TClosingDateStr = string.Format("{0:dd.MM.yyyy}", d.TClosingDate),
                        HasCmrDeliveryed = d.HasCmrDeliveryed,
                        ItemPrice = d.ItemPrice,
                        TrailerType = d.TrailerType,
                        HasItemInsurance = d.HasItemInsurance,
                        HasItemDangerous = d.HasItemDangerous,
                        //CmrCustomerDeliveryDateStr = string.Format("{0:dd.MM.yyyy}", d.CmrCustomerDeliveryDate),
                        BringingToWarehousePlate = d.BringingToWarehousePlate,
                        ShipperCityId = d.ShipperCityId,
                        BuyerCityId = d.BuyerCityId,
                        ShipperCountryId = d.ShipperCountryId,
                        BuyerCountryId = d.BuyerCountryId,
                        CustomerFirmId = d.CustomerFirmId,
                        ShipperFirmId = d.ShipperFirmId,
                        BuyerFirmId = d.BuyerFirmId,
                        EntryCustomsId = d.EntryCustomsId,
                        ExitCustomsId = d.ExitCustomsId,
                        PlantId = d.PlantId,
                        RotaId = d.RotaId,
                    }).ToArray();
            }

            return model;
        }

        public VoyageModel[] GetVoyageList()
        {
            List<VoyageModel> data = new List<VoyageModel>();

            var repo = _unitOfWork.GetRepository<Voyage>();

            repo.GetAll().ToList().ForEach(d =>
            {
                VoyageModel containerObj = new VoyageModel();
                d.MapTo(containerObj);
                    containerObj.Id = d.Id;
                    containerObj.VoyageCode = d.VoyageCode;
                    containerObj.VoyageDateStr = string.Format("{0:dd.MM.yyyy}", d.VoyageDate);
                    containerObj.VoyageStatusStr = d.VoyageStatus != null ? ((VoyageStatus)d.VoyageStatus).ToCaption() : "";
                    containerObj.CarrierFirmName = d.CarrierFirm != null ? d.CarrierFirm.FirmName : "";
                    containerObj.CustomsDoorEntryDateStr = string.Format("{0:dd.MM.yyyy}", d.CustomsDoorEntryDate);
                    containerObj.CustomsDoorEntryName = d.CustomsDoorEntry != null ? d.CustomsDoorEntry.CustomsDoorName : "";
                    containerObj.CustomsDoorExitDateStr = string.Format("{0:dd.MM.yyyy}", d.CustomsDoorExitDate);
                    containerObj.CustomsDoorExitName = d.CustomsDoorExit != null ? d.CustomsDoorExit.CustomsDoorName : "";
                    containerObj.DriverNameAndSurname = d.Driver != null ? d.Driver.DriverName + " " + d.Driver.DriverSurName : "";
                    containerObj.DriverSubsistence = d.DriverSubsistence;
                    containerObj.EmptyGo = d.EmptyGo;
                    containerObj.EndDateStr = string.Format("{0:dd.MM.yyyy}", d.EndDate);
                    containerObj.StartDateStr = string.Format("{0:dd.MM.yyyy}", d.StartDate);
                    containerObj.LoadDateStr = string.Format("{0:dd.MM.yyyy}", d.LoadDate);
                    containerObj.Explanation = d.Explanation;
                    containerObj.OrderTransactionDirectionTypeStr = d.OrderTransactionDirectionType != null ? ((OrderTransactionDirectionType)d.OrderTransactionDirectionType).ToCaption() : "";
                    containerObj.OverallQuantity = d.OverallQuantity;
                    containerObj.OverallVolume = d.OverallVolume;
                    containerObj.OverallWeight = d.OverallWeight;
                    containerObj.OverallGrossWeight = d.OverallGrossWeight;
                    containerObj.PositionKmHour = d.PositionKmHour;
                    containerObj.TowinfVehiclePlate = d.TowinfVehicle != null ? d.TowinfVehicle.Plate : "";
                    containerObj.TowinfVehicleMarkAndModel = d.TowinfVehicle != null ? d.TowinfVehicle.Mark + " " + d.TowinfVehicle.Versiyon : "";
                    containerObj.TraillerVehiclePlate = d.TraillerVehicle != null ? d.TraillerVehicle.Plate : "";
                    containerObj.TraillerVehicleMarkAndModel = d.TraillerVehicle != null ? d.TraillerVehicle.Mark + " " + d.TraillerVehicle.Versiyon : "";
                    containerObj.VehicleExitDateStr = string.Format("{0:dd.MM.yyyy}", d.VehicleExitDate);
                    containerObj.ForexTypeCode = d.ForexType != null ? d.ForexType.ForexTypeCode : "";
                    data.Add(containerObj);
            });

            return data.ToArray();
        }
        public VoyageDetailModel[] GetVoyageDetailList()
        {
            var repo = _unitOfWork.GetRepository<VoyageDetail>();

            return repo.GetAll()
                .Select(d => new VoyageDetailModel
                {
                    Id = d.Id,
                    VoyageCode = d.Voyage != null ? d.Voyage.VoyageCode :"",
                    VoyageStatusStr = d.VoyageStatus != null ? ((LoadStatusType)d.VoyageStatus).ToCaption() : "",
                    DischargeDateStr = string.Format("{0:dd.MM.yyyy}", d.DischargeDate),
                    LoadingDateStr = string.Format("{0:dd.MM.yyyy}", d.LoadingDate),
                    DateOfNeedStr = string.Format("{0:dd.MM.yyyy}", d.DateOfNeed),
                    LoadOutDateStr = string.Format("{0:dd.MM.yyyy}", d.LoadOutDate),
                    ScheduledUploadDateStr = string.Format("{0:dd.MM.yyyy}", d.ScheduledUploadDate),
                    CustomerFirmName = d.FirmCustomer != null ? d.FirmCustomer.FirmName : "",
                    EntryCustomsName = d.CustomsEntry != null ? d.CustomsEntry.CustomsName : "",
                    ExitCustomsName = d.CustomsExit != null ? d.CustomsExit.CustomsName : "",
                    ShipperFirmName = d.FirmShipper != null ? d.FirmShipper.FirmName : "",
                    BuyerFirmName = d.FirmBuyer != null ? d.FirmBuyer.FirmName : "",
                    OrderTransactionDirectionTypeStr = d.OrderTransactionDirectionType != null ? ((OrderTransactionDirectionType)d.OrderTransactionDirectionType).ToCaption() : "",
                    OrderUploadTypeStr = d.OrderUploadType == 1 ? LSabit.GET_GRUPAJ : d.OrderUploadType == 2 ? LSabit.GET_COMPLATE : "",
                    OrderUploadPointTypeStr = d.OrderUploadPointType == 1 ? LSabit.GET_FROMCUSTOMER : d.OrderUploadPointType == 2 ? LSabit.GET_FROMWAREHOUSE : "",
                    OrderCalculationTypeStr = d.OrderCalculationType == 1 ? LSabit.GET_WEIGHTTED : d.OrderCalculationType == 2 ? LSabit.GET_VOLUMETRIC : d.OrderCalculationType == 3 ? LSabit.GET_LADAMETRE : d.OrderCalculationType == 4 ? LSabit.GET_COMPLET : d.OrderCalculationType == 5 ? LSabit.GET_MINIMUM : "",
                    ShipperCityName = d.CityShipper != null ? d.CityShipper.CityName : "",
                    BuyerCityName = d.CityBuyer != null ? d.CityBuyer.CityName : "",
                    ShipperCountryName = d.CountryShipper != null ? d.CountryShipper.CountryName : "",
                    BuyerCountryName = d.CountryBuyer != null ? d.CountryBuyer.CountryName : "",
                    ShipperFirmExplanation = d.ShipperFirmExplanation,
                    BuyerFirmExplanation = d.BuyerFirmExplanation,
                    ForexTypeCode = d.ForexType != null ? d.ForexType.ForexTypeCode : "",
                    RotaEndCityPostCode = d.Rota != null ? d.Rota.CityEnd !=null ? d.Rota.CityEnd.PostCode:"":"",
                    RotaEndCityName = d.Rota != null ? d.Rota.CityEnd != null ? d.Rota.CityEnd.CityName : "" : "",
                    RotaStartCityPostCode = d.Rota != null ? d.Rota.CityStart != null ? d.Rota.CityStart.PostCode : "" : "",
                    RotaStartCityName = d.Rota != null ? d.Rota.CityStart != null ? d.Rota.CityStart.CityName : "" : "",
                    DischargeLineNo = d.DischargeLineNo
                }).ToArray();
        }

        //public BusinessResult DeleteVoyage(int id)
        //{
        //    BusinessResult result = new BusinessResult();

        //    try
        //    {
        //        var repoVoage = _unitOfWork.GetRepository<Voyage>();
        //        var repoVoyageDetail = _unitOfWork.GetRepository<VoyageDetail>();
        //        var repoNotify = _unitOfWork.GetRepository<Notification>();

        //        var dbObj = repoVoage.Get(d => d.Id == id);
        //        if (dbObj == null)
        //            throw new Exception("Silinmesi istenen sefer kaydına ulaşılamadı.");

        //        //TODO: Sefere Dönüştürülmüş Yük silinemez
        //        //if (dbObj.ItemReceipt.Any())
        //        //    throw new Exception("İrsaliyesi girilmiş olan bir sipariş silinemez.");

        //        // CLEAR DETAILS
        //        if (dbObj.ItemLoadDetail.Any())
        //        {
        //            var detailObjArr = dbObj.ItemLoadDetail.ToArray();
        //            foreach (var item in detailObjArr)
        //            {
        //                #region SET REQUEST & DETAIL TO APPROVED
        //                if (item.ItemOrderDetail != null)
        //                {
        //                    item.ItemOrderDetail.OrderStatus = (int)OrderStatusType.Approved;
        //                    item.ItemOrderDetail.ItemOrder.OrderStatus = (int)RequestStatusType.Approved;
        //                }
        //                #endregion

        //                repoDetail.Delete(item);
        //            }
        //        }

        //        //// CLEAR NEEDS
        //        //if (dbObj.ItemOrderItemNeeds.Any())
        //        //{
        //        //    var needs = dbObj.ItemOrderItemNeeds.ToArray();
        //        //    foreach (var needItem in needs)
        //        //    {
        //        //        repoNeeds.Delete(needItem);
        //        //    }
        //        //}

        //        // CLEAR NOTIFICATIONS
        //        if (repoNotify.Any(d => d.NotifyType == (int)NotifyType.ItemOrderWaitForApproval && d.RecordId == dbObj.Id))
        //        {
        //            var notificationArr = repoNotify.Filter(d => d.NotifyType == (int)NotifyType.ItemOrderWaitForApproval && d.RecordId == dbObj.Id)
        //                .ToArray();

        //            foreach (var item in notificationArr)
        //            {
        //                repoNotify.Delete(item);
        //            }
        //        }

        //        repo.Delete(dbObj);
        //        _unitOfWork.SaveChanges();

        //        result.Result = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Result = false;
        //        result.ErrorMessage = ex.Message;
        //    }

        //    return result;
        //}
    }
}
