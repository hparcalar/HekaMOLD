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
        public string GetVoyageCode(string param)
        {
            string[] Request = param.Split('-');
            string defaultValue = "";
            var directionId = Convert.ToInt32(Request[0]);
            var vehicleAllocationType = Convert.ToInt32(Request[1]);

            try
            {

                defaultValue =  GetPrefixOwnOrRental(vehicleAllocationType, directionId) + ((OrderTransactionDirectionType)directionId).ToCaption();
                return defaultValue;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string GetPrefixOwnOrRental(int vehicleAllocationType, int directionId)
        {
            var repo = _unitOfWork.GetRepository<CodeCounter>();
            var dbCodeCounter = repo.Filter(d => d.CounterType == 3)// CounterType type=1(Order) type=2(Load) type=3(Voyage)
                .OrderByDescending(d => d.Id)
                .Select(d => d)
                .FirstOrDefault();

            string value = "";
            if (vehicleAllocationType == (int)VehicleAllocationType.Own && directionId == LSabit.GET_OWN_EXPORT)
                value = "OZ";
            else if (vehicleAllocationType == (int)VehicleAllocationType.ForRent && directionId == LSabit.GET_OWN_EXPORT)
                value = "KR";
            else if (vehicleAllocationType == (int)VehicleAllocationType.ForRent && directionId == LSabit.GET_OWN_IMPORT)
                value = "KR";
            else
                value = "00";

            if (directionId == (int)(OrderTransactionDirectionType.Export) && vehicleAllocationType == (int)VehicleAllocationType.Own)
                return dbCodeCounter.FirstValue + value + string.Format("{0:0000}", Convert.ToInt32((int)dbCodeCounter.OwnExport + 1));

            if (directionId == (int)(OrderTransactionDirectionType.Import) && vehicleAllocationType == (int)VehicleAllocationType.Own)
                return dbCodeCounter.FirstValue + value + string.Format("{0:0000}", Convert.ToInt32((int)dbCodeCounter.OwnImport + 1));

            if (directionId == (int)(OrderTransactionDirectionType.Domestic) && vehicleAllocationType == (int)VehicleAllocationType.Own)
                return dbCodeCounter.FirstValue + value + string.Format("{0:0000}", Convert.ToInt32((int)dbCodeCounter.OwnDomestic + 1));

            if (directionId == (int)(OrderTransactionDirectionType.Transit) && vehicleAllocationType == (int)VehicleAllocationType.Own)
                return dbCodeCounter.FirstValue + value + string.Format("{0:0000}", Convert.ToInt32((int)dbCodeCounter.OwnTransit + 1));

            if (directionId == (int)(OrderTransactionDirectionType.Export) && vehicleAllocationType == (int)VehicleAllocationType.ForRent)
                return dbCodeCounter.FirstValue + value + string.Format("{0:0000}", Convert.ToInt32((int)dbCodeCounter.RentalExport + 1));

            if (directionId == (int)(OrderTransactionDirectionType.Import) && vehicleAllocationType == (int)VehicleAllocationType.ForRent)
                return dbCodeCounter.FirstValue + value + string.Format("{0:0000}", Convert.ToInt32((int)dbCodeCounter.RentalImport + 1));

            if (directionId == (int)(OrderTransactionDirectionType.Domestic) && vehicleAllocationType == (int)VehicleAllocationType.ForRent)
                return dbCodeCounter.FirstValue + value + string.Format("{0:0000}", Convert.ToInt32((int)dbCodeCounter.RentalDomestic + 1));
            else
                return dbCodeCounter.FirstValue + value + string.Format("{0:0000}", Convert.ToInt32((int)dbCodeCounter.RentalTransit + 1));

        }
        public string GetReservationVoyageCode(string voyageExportCode)
        {
            string voyageImportCode = "";
            voyageImportCode = voyageExportCode.Substring(0, voyageExportCode.Length - 2);
            return voyageImportCode + "IM";
        }

        public BusinessResult SaveOrUpdateVoyage(VoyageModel model, int userId, bool detailCanBeNull = false)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repoVoyage = _unitOfWork.GetRepository<Voyage>();
                var repoVoyageDriver = _unitOfWork.GetRepository<VoyageDriver>();
                var repoVoyageTowingVehicle = _unitOfWork.GetRepository<VoyageTowingVehicle>();
                var repoVoyageDetail = _unitOfWork.GetRepository<VoyageDetail>();
                var repoLoad = _unitOfWork.GetRepository<ItemLoad>();
                var repoLoadDetail = _unitOfWork.GetRepository<ItemLoadDetail>();
                var repoNotify = _unitOfWork.GetRepository<Notification>();
                var repoCodeCounter = _unitOfWork.GetRepository<CodeCounter>();
                var repoVehicle = _unitOfWork.GetRepository<Vehicle>();

                if (model.OrderTransactionDirectionType == null)
                    throw new Exception("İşlem yönü seçilmelidir !");

                if (model.TraillerVehicleId == null)
                    throw new Exception("Romörk seçilmelidir !");

                bool newRecord = false;
                var dbObj = repoVoyage.Get(d => d.Id == model.Id);
                if (dbObj == null)
                {
                    dbObj = new Voyage();
                    //dbObj.VoyageCode = GetVoyageCode((int)model.OrderTransactionDirectionType +"-"+ model.Tra);
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
                if (!string.IsNullOrEmpty(model.FirstLoadDateStr))
                {
                    model.FirstLoadDate = DateTime.ParseExact(model.FirstLoadDateStr, "dd.MM.yyyy",
                        System.Globalization.CultureInfo.GetCultureInfo("tr"));
                }
                if (!string.IsNullOrEmpty(model.EndDischargeDateStr))
                {
                    model.EndDischargeDate = DateTime.ParseExact(model.EndDischargeDateStr, "dd.MM.yyyy",
                        System.Globalization.CultureInfo.GetCultureInfo("tr"));
                }
                if (!string.IsNullOrEmpty(model.KapikulePassportEntryDateStr))
                {
                    model.KapikulePassportEntryDate = DateTime.ParseExact(model.KapikulePassportEntryDateStr, "dd.MM.yyyy",
                        System.Globalization.CultureInfo.GetCultureInfo("tr"));
                }
                if (!string.IsNullOrEmpty(model.KapikulePassportExitDateStr))
                {
                    model.KapikulePassportExitDate = DateTime.ParseExact(model.KapikulePassportExitDateStr, "dd.MM.yyyy",
                        System.Globalization.CultureInfo.GetCultureInfo("tr"));
                }
                //else if (string.IsNullOrEmpty(model.VoyageDateStr))
                //    throw new Exception("Sefer tarihi bilgisini giriniz !");

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

                #region SAVE DRIVERS_AND_TOWINGVEHECILE
                if (model.VoyageDrivers == null)
                    model.VoyageDrivers = new VoyageDriverModel[0];

                var toBeRemovedDrivers = dbObj.VoyageDriver
                    .Where(d => !model.VoyageDrivers.Where(m => m.NewDetail == false)
                        .Select(m => m.Id).ToArray().Contains(d.Id)
                    ).ToArray();
                foreach (var item in toBeRemovedDrivers)
                {
                    repoVoyageDriver.Delete(item);
                }
                foreach (var item in model.VoyageDrivers)
                {
                    //if (!string.IsNullOrEmpty(item.StartDateStr))
                    //{
                    //    item.StartDate = DateTime.ParseExact(item.StartDateStr, "dd/MM/yyyy",
                    //        System.Globalization.CultureInfo.GetCultureInfo("tr"));
                    //}
                    //if (!string.IsNullOrEmpty(item.EndDateStr))
                    //{
                    //    item.EndDate = DateTime.ParseExact(item.EndDateStr, "dd.MM.yyyy",
                    //        System.Globalization.CultureInfo.GetCultureInfo("tr"));
                    //}

                    if (item.NewDetail == true)
                    {
                        var dbVoyageDriver = new VoyageDriver();
                        item.MapTo(dbVoyageDriver);
                        dbVoyageDriver.Voyage = dbObj;
                        repoVoyageDriver.Add(dbVoyageDriver);
                    }
                    else if (!toBeRemovedDrivers.Any(d=>d.Id == item.Id))
                    {
                        var dbVoyageDriver = repoVoyageDriver.GetById(item.Id);
                        item.MapTo(dbVoyageDriver);
                        dbVoyageDriver.Voyage = dbObj;
                    }
                }
                #endregion
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
                        dbItemLoad.TowinfVehicleId = model.TowinfVehicleId;
                        dbItemLoad.DischargeLineNo = x.DischargeLineNo;
                        dbItemLoad.LoadingLineNo = x.LoadingLineNo;
                        dbItemLoad.TrailerType = model.TraillerType;
                        dbItemLoad.VoyageExitDate = model.StartDate;
                        dbItemLoad.VoyageEndDate = model.EndDate;
                        dbItemLoad.DriverId = model.DriverId;
                        //x.MapTo(dbItemLoad);
                    }

                }

                if ( dbObj.VoyageStatus == (int)VoyageStatus.Cancelled)
                {

                    foreach (var x in model.VoyageDetails)
                    {
                        var dbItemLoad = repoLoad.Get(d => d.Id == x.ItemLoadId);

                        dbItemLoad.LoadStatusType = (int)LoadStatusType.Ready;
                        dbItemLoad.VoyageCode = "";
                        dbItemLoad.VoyageCreatedUserId =null;
                        dbItemLoad.VehicleTraillerId = null;
                        //x.MapTo(dbItemLoad);
                    }

                }
                if (dbObj.VoyageStatus != (int)VoyageStatus.Cancelled)
                {

                    foreach (var x in model.VoyageDetails)
                    {
                        var dbItemLoad = repoLoad.Get(d => d.Id == x.ItemLoadId);

                        if(dbObj.VoyageStatus != (int)VoyageStatus.Created && dbObj.VoyageStatus != (int)VoyageStatus.Approved && dbObj.VoyageStatus != (int)VoyageStatus.Ready)
                            dbItemLoad.LoadStatusType = model.VoyageStatus;
                        dbItemLoad.VoyageCode = model.VoyageCode;
                        dbItemLoad.VoyageCreatedUserId = model.CreatedUserId;
                        dbItemLoad.VehicleTraillerId = model.TraillerVehicleId;
                        dbItemLoad.TowinfVehicleId = model.TowinfVehicleId;
                        dbItemLoad.DischargeLineNo = x.DischargeLineNo;
                        dbItemLoad.LoadingLineNo = x.LoadingLineNo;
                        dbItemLoad.TrailerType = model.TraillerType;
                        dbItemLoad.VoyageExitDate = model.StartDate;
                        dbItemLoad.VoyageEndDate = model.EndDate;
                        dbItemLoad.DriverId = model.DriverId;
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
                #region SET RESERVATION VOYAGE
                var dbVehicle = repoVehicle.Get(d => d.Id == model.TraillerVehicleId);
                var ringCode = "";
                if ( newRecord && model.OrderTransactionDirectionType == 1 && dbVehicle.VehicleAllocationType == 1)
                {
                    VoyageModel voyageModel = new VoyageModel();
                    voyageModel.RingCode = model.VoyageCode;
                    voyageModel.OrderTransactionDirectionType = ((int)OrderTransactionDirectionType.Import);
                    voyageModel.TraillerVehicleId = model.TraillerVehicleId;
                    ringCode = GetReservationVoyageCode(model.VoyageCode);
                    SaveOrUpdateReservationVoyage(voyageModel, userId, false);
                }
                #endregion
                model.RingCode = ringCode;
                #region CODECOUNTER
                var objCodeCounter = repoCodeCounter.Filter(d => d.CounterType == 3)
                    .OrderByDescending(d => d.Id)
                    .Select(d => d)
                    .FirstOrDefault();
                var dbrepoCodeCounter = repoCodeCounter.Get(d => d.Id == objCodeCounter.Id);
                if (newRecord)
                {
                    var dbVehicleObj = repoVehicle.Get(d => d.Id == model.TraillerVehicleId);

                    if (model.OrderTransactionDirectionType == 1 && dbVehicleObj.VehicleAllocationType == (int)VehicleAllocationType.Own)
                    {
                        dbrepoCodeCounter.OwnExport++;
                    }
                    if (model.OrderTransactionDirectionType == 2 && dbVehicleObj.VehicleAllocationType == (int)VehicleAllocationType.Own)
                    {
                        dbrepoCodeCounter.OwnImport++;
                    }
                    if (model.OrderTransactionDirectionType == 3 && dbVehicleObj.VehicleAllocationType == (int)VehicleAllocationType.Own)
                    {
                        dbrepoCodeCounter.OwnDomestic++;
                    }
                    if (model.OrderTransactionDirectionType == 4 && dbVehicleObj.VehicleAllocationType == (int)VehicleAllocationType.Own)
                    {
                        dbrepoCodeCounter.OwnTransit++;
                    }
                    if (model.OrderTransactionDirectionType == 1 && dbVehicleObj.VehicleAllocationType == (int)VehicleAllocationType.ForRent)
                    {
                        dbrepoCodeCounter.RentalExport++;
                    }
                    if (model.OrderTransactionDirectionType == 2 && dbVehicleObj.VehicleAllocationType == (int)VehicleAllocationType.ForRent)
                    {
                        dbrepoCodeCounter.RentalImport++;
                    }
                    if (model.OrderTransactionDirectionType == 3 && dbVehicleObj.VehicleAllocationType == (int)VehicleAllocationType.ForRent)
                    {
                        dbrepoCodeCounter.RentalDomestic++;
                    }
                    if (model.OrderTransactionDirectionType == 4 && dbVehicleObj.VehicleAllocationType == (int)VehicleAllocationType.ForRent)
                    {
                        dbrepoCodeCounter.RentalTransit++;
                    }
                }

                #endregion
                if (model.VoyageStatus == (int)VoyageStatus.Completed)
                {
                    var dbResevationVoyage = repoVoyage.Get(d=>d.RingCode == model.VoyageCode);
                    dbResevationVoyage.StartDate = model.EndDate;
                    dbResevationVoyage.StartCityId = model.DischargeCityId;
                    dbResevationVoyage.StartCountryId = model.DischargeCountryId;

                }
                _unitOfWork.SaveChanges();
                #region CREATE NOTIFICATION
                if (newRecord && !repoNotify.Any(d => d.RecordId == dbObj.Id && d.NotifyType == (int)NotifyType.VoyageWaitForApproval))
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
        public BusinessResult SaveOrUpdateReservationVoyage(VoyageModel model, int userId, bool detailCanBeNull = false)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repoVoyage = _unitOfWork.GetRepository<Voyage>();
                var repoNotify = _unitOfWork.GetRepository<Notification>();
                var repoCodeCounter = _unitOfWork.GetRepository<CodeCounter>();



                bool newRecord = false;
                var dbObj = repoVoyage.Get(d => d.Id == model.Id);
                if (dbObj == null)
                {
                    dbObj = new Voyage();
                    dbObj.VoyageCode = GetReservationVoyageCode(model.RingCode);
                    dbObj.CreatedDate = DateTime.Now;
                    dbObj.CreatedUserId = userId;
                    dbObj.VoyageStatus = (int)VoyageStatus.Created;
                    repoVoyage.Add(dbObj);
                    newRecord = true;
                }



                var crDate = dbObj.CreatedDate;
                var reqStats = dbObj.VoyageStatus;
                var crUserId = dbObj.CreatedUserId;
                var voyageCode = GetReservationVoyageCode(model.RingCode);

                model.MapTo(dbObj);

                if (dbObj.CreatedDate == null)
                    dbObj.CreatedDate = crDate;
                if (dbObj.VoyageStatus == null)
                    dbObj.VoyageStatus = reqStats;
                if (dbObj.CreatedUserId == null)
                    dbObj.CreatedUserId = crUserId;
                if (dbObj.VoyageCode == null)
                    dbObj.VoyageCode = voyageCode;
                dbObj.UpdatedDate = DateTime.Now;


                //#region CODECOUNTER
                //var objCodeCounter = repoCodeCounter.Filter(d => d.CounterType == 3)
                //    .OrderByDescending(d => d.Id)
                //    .Select(d => d)
                //    .FirstOrDefault();
                //var dbrepoCodeCounter = repoCodeCounter.Get(d => d.Id == objCodeCounter.Id);
                //if (newRecord)
                //{
                //    if (model.OrderTransactionDirectionType == 1)
                //    {
                //        dbrepoCodeCounter.Export++;
                //    }
                //    if (model.OrderTransactionDirectionType == 2)
                //    {
                //        dbrepoCodeCounter.Import++;
                //    }
                //    if (model.OrderTransactionDirectionType == 3)
                //    {
                //        dbrepoCodeCounter.Domestic++;
                //    }
                //    if (model.OrderTransactionDirectionType == 4)
                //    {
                //        dbrepoCodeCounter.Transit++;
                //    }
                //}
                //#endregion
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
            var repoDrivers = _unitOfWork.GetRepository<VoyageDriver>();
            var repoTowingVehicles = _unitOfWork.GetRepository<VoyageTowingVehicleModel>();

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
                model.FirstLoadDateStr = string.Format("{0:dd.MM.yyyy}", dbObj.FirstLoadDate);
                model.EndDischargeDateStr = string.Format("{0:dd.MM.yyyy}", dbObj.EndDischargeDate);
                model.KapikulePassportEntryDateStr = string.Format("{0:dd.MM.yyyy}", dbObj.KapikulePassportEntryDate);
                model.KapikulePassportExitDateStr = string.Format("{0:dd.MM.yyyy}", dbObj.KapikulePassportExitDate);
                model.VoyageDetails =
                    repoDetails.Filter(d => d.VoyageId == dbObj.Id)
                    .Select(d => new VoyageDetailModel
                    {
                        Id = d.Id,
                        DischargeLineNo = d.DischargeLineNo,
                        ItemLoadId = d.ItemLoadId,
                        LoadCode = d.LoadCode,
                        LoadingLineNo = d.LoadingLineNo,
                        //LoadingDateStr = string.Format("{0:dd.MM.yyyy}", d.LoadingDate),
                        //LoadOutDateStr = string.Format("{0:dd.MM.yyyy}", d.LoadOutDate),
                        //OrderTransactionDirectionTypeStr = d.OrderTransactionDirectionType != null ? ((OrderTransactionDirectionType)dbObj.OrderTransactionDirectionType).ToCaption() : "",
                        OveralQuantity = d.OveralQuantity,
                        OveralWeight = d.OveralWeight,
                        OveralLadametre = d.OveralLadametre,
                        OveralVolume = d.OveralVolume,
                        OverallTotal = d.OverallTotal,
                        OrderNo = d.OrderNo,
                        //VoyageStatusStr = d.VoyageStatus !=null ? ((VoyageStatus)model.VoyageStatus).ToCaption():"",
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
                        ShipperCityName = d.CityShipper != null ? d.CityShipper.PlateCode +"/"+ d.CityShipper.CityName:"",
                        BuyerCityId = d.BuyerCityId,
                        BuyerCityName = d.CityBuyer != null ? d.CityBuyer.PostCode + "/" + d.CityBuyer.CityName : "",
                        ShipperCountryId = d.ShipperCountryId,
                        ShipperCountryName = d.CountryShipper != null ? d.CountryShipper.CountryName : "",
                        BuyerCountryId = d.BuyerCountryId,
                        BuyerCountryName = d.CountryBuyer != null ? d.CountryBuyer.CountryName : "",
                        CustomerFirmId = d.CustomerFirmId,
                        ShipperFirmId = d.ShipperFirmId,
                        BuyerFirmId = d.BuyerFirmId,
                        BuyerFirmName = d.FirmBuyer != null ? d.FirmBuyer.FirmName:"",
                        ShipperFirmName = d.FirmShipper != null ? d.FirmShipper.FirmName : "",
                        CustomerFirmName = d.FirmCustomer != null ? d.FirmCustomer.FirmName:"",
                        EntryCustomsId = d.EntryCustomsId,
                        ExitCustomsId = d.ExitCustomsId,
                        PlantId = d.PlantId,
                        RotaId = d.RotaId,
                    }).ToArray();
                model.VoyageDrivers =
                    repoDrivers.Filter(d => d.VoyageId == dbObj.Id)
                    .Select(d => new VoyageDriverModel
                    {
                        Id = d.Id,
                        DriverId = d.DriverId,
                        StartDate = d.StartDate,
                        //StartDate = string.Format("{0:dd.MM.yyyy}", d.StartDate),
                        //EndDateStr = string.Format("{0:dd.MM.yyyy}", d.EndDate),
                        StartKmHour = d.StartKmHour,
                        EndKmHour = d.EndKmHour,
                        TowingVehicleId = d.TowingVehicleId

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
        public int GetNextRecord(int plantId, int Id)
        {
            try
            {
                var repo = _unitOfWork.GetRepository<Voyage>();
                int lastVoyageNo = repo.Filter(d => d.PlantId == plantId && d.Id > Id)
                    .OrderBy(d => d.Id)
                    .Select(d => d.Id)
                    .FirstOrDefault();

                if (string.IsNullOrEmpty(Convert.ToString(lastVoyageNo)))
                    lastVoyageNo = 0;

                return lastVoyageNo;
            }
            catch (Exception)
            {

            }

            return default;
        }
        public int GetBackRecord(int plantId, int Id)
        {
            try
            {
                var repo = _unitOfWork.GetRepository<Voyage>();
                int lastVoyageNo = repo.Filter(d => d.PlantId == plantId && d.Id < Id)
                    .OrderByDescending(d => d.Id)
                    .Select(d => d.Id)
                    .FirstOrDefault();

                if (string.IsNullOrEmpty(Convert.ToString(lastVoyageNo)))
                    lastVoyageNo = 0;

                return lastVoyageNo;
            }
            catch (Exception)
            {

            }

            return default;
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

        #region VOYAGECOST
        public VoyageCostModel GetVoyageCost(int id, int vid)
        {
            VoyageCostModel model = new VoyageCostModel { VoyageCostDetails = new VoyageCostDetailModel[0] };

            var repo = _unitOfWork.GetRepository<VoyageCost>();
            var repoDetails = _unitOfWork.GetRepository<VoyageCostDetail>();
            var repoVoyage = _unitOfWork.GetRepository<Voyage>();

            var dbObj = repo.Get(d => d.Id == id);
            var dbVoyageObj = repoVoyage.Get(d => d.Id == vid);
            if (dbObj==null && dbVoyageObj != null)
            {
                model.VoyageCode = dbVoyageObj.VoyageCode;
                model.VoyageStatusStr = ((VoyageStatus)dbVoyageObj.VoyageStatus).ToCaption();
                model.TrailerPlate = dbVoyageObj.TraillerVehicle != null ? dbVoyageObj.TraillerVehicle.Plate : "";
                model.OrderTransationDirectionTypeStr = dbVoyageObj.OrderTransactionDirectionType != null ? (int)dbVoyageObj.OrderTransactionDirectionType == 1 ? LSabit.GET_EXPORT.ToString() : (int)dbVoyageObj.OrderTransactionDirectionType == 2 ? LSabit.GET_IMPORT.ToString() : (int)dbVoyageObj.OrderTransactionDirectionType == 3 ? LSabit.GET_DOMESTIC.ToString() : (int)dbVoyageObj.OrderTransactionDirectionType == 4 ? LSabit.GET_TRASFER.ToString() : "" : "";
                model.VoyageId = dbVoyageObj.Id;
            }
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);
                model.VoyageCode = dbObj.Voyage != null ? dbObj.Voyage.VoyageCode : "";
                model.VoyageEndDateStr = string.Format("{0:dd.MM.yyyy}", dbObj.Voyage.EndDate);
                model.VoyageStatusStr = ((VoyageStatus)dbObj.Voyage.VoyageStatus).ToCaption();
                model.VoyageStartDateStr = string.Format("{0:dd.MM.yyyy}", dbObj.Voyage.StartDate);
                model.TrailerPlate = dbObj.Voyage.TraillerVehicle != null ? dbObj.Voyage.TraillerVehicle.Plate : "";
                model.OrderTransationDirectionTypeStr = dbObj.Voyage.OrderTransactionDirectionType != null ? (int)dbObj.Voyage.OrderTransactionDirectionType == 1 ? LSabit.GET_EXPORT.ToString() : (int)dbObj.Voyage.OrderTransactionDirectionType == 2 ? LSabit.GET_IMPORT.ToString(): (int)dbObj.Voyage.OrderTransactionDirectionType == 3 ? LSabit.GET_DOMESTIC.ToString(): (int)dbObj.Voyage.OrderTransactionDirectionType == 4 ? LSabit.GET_TRASFER.ToString():"" :"";
                model.VoyageCostDetails =
                    repoDetails.Filter(d => d.VoyageCostId == dbObj.Id)
                    .Select(d => new VoyageCostDetailModel
                    {
                        Id = d.Id,
                        CostCategoryId = d.CostCategoryId,
                        CostCategoryName = d.CostCategory != null ? d.CostCategory.CostCategoryName:"",
                        CountryId = d.CountryId,
                        CountryName = d.Country != null ? d.Country.CountryName:"",
                        DriverId = d.DriverId,
                        DriverNameAndSurName = d.Driver != null ?  d.Driver.DriverName +"/"+d.Driver.DriverSurName:"",
                        ForexTypeCode = d.ForexType != null ? d.ForexType.ForexTypeCode :"",
                        ForexTypeId = d.ForexTypeId,
                        UnitTypeId = d.UnitTypeId,
                        UnitCode = d.UnitType !=null ? d.UnitType.UnitCode:"",
                        Quantity = d.Quantity,
                        OverallTotal = d.OverallTotal,
                        PayType = d.PayType,
                        PayTypeStr = d.PayType == LSabit.GET_VOYAGECOST_CASH ? "Peşin" :"Kredili",
                        TowingVehicleId = d.TowingVehicleId,
                        TowingVehiclePlate = d.Vehicle !=null ? d.Vehicle.Plate : "",
                        TowingVehicleMarkAndVersiyon = d.Vehicle !=null ? d.Vehicle.Mark +"/"+ d.Vehicle.Versiyon:"",
                        KmHour = d.KmHour,
                        ActionType = d.ActionType
             
                    }).ToArray();
            }

            return model;
        }
        public VoyageCostModel[] GetVoyageCostList()
        {
            List<VoyageCostModel> data = new List<VoyageCostModel>();

            var repo = _unitOfWork.GetRepository<VoyageCost>();

            repo.GetAll().ToList().ForEach(d =>
            {
                VoyageCostModel containerObj = new VoyageCostModel();
                d.MapTo(containerObj);
                containerObj.Id = d.Id;
                containerObj.VoyageId = d.VoyageId;
                data.Add(containerObj);
            });

            return data.ToArray();
        }
        public VoyageCostModel GetVoyageCostByVoyage(int id)
        {
            VoyageCostModel model = new VoyageCostModel { VoyageCostDetails = new VoyageCostDetailModel[0] };

            var repo = _unitOfWork.GetRepository<VoyageCost>();
            var repoDetails = _unitOfWork.GetRepository<VoyageCostDetail>();

            var dbObj = repo.Get(d => d.VoyageId == id);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);

                model.VoyageCostDetails =
                    repoDetails.Filter(d => d.VoyageCostId == dbObj.Id)
                    .Select(d => new VoyageCostDetailModel
                    {
                        Id = d.Id,
                        CostCategoryId = d.CostCategoryId,
                        CostCategoryName = d.CostCategory != null ? d.CostCategory.CostCategoryName : "",
                        CountryId = d.CountryId,
                        CountryName = d.Country != null ? d.Country.CountryName : "",
                        DriverId = d.DriverId,
                        DriverNameAndSurName = d.Driver != null ? d.Driver.DriverName + "/" + d.Driver.DriverSurName : "",
                        ForexTypeCode = d.ForexType != null ? d.ForexType.ForexTypeCode : "",
                        Quantity = d.Quantity,
                        OverallTotal = d.OverallTotal,
                        PayTypeStr = d.PayType == LSabit.GET_VOYAGECOST_CASH ? "Peşin" : "Kredili",

                    }).ToArray();
            }

            return model;
        }
        public BusinessResult SaveOrUpdateVoyageCost(VoyageCostModel model, int userId, bool detailCanBeNull = false)
        {
            BusinessResult result = new BusinessResult();

            try
            {

                var repo = _unitOfWork.GetRepository<VoyageCost>();
                var repoDetail = _unitOfWork.GetRepository<VoyageCostDetail>();
                var repoDriverAccountDetail = _unitOfWork.GetRepository<DriverAccountDetail>();
                var repoDriverAccount = _unitOfWork.GetRepository<DriverAccount>();


                var dbObj = repo.Get(d => d.Id == model.Id);
                if (dbObj == null)
                {
                    dbObj = new VoyageCost();
                    dbObj.CreatedDate = DateTime.Now;
                    dbObj.CreatedUserId = model.CreatedUserId;
                    repo.Add(dbObj);
                }

                var crDate = dbObj.CreatedDate;

                model.MapTo(dbObj);

                if (dbObj.CreatedDate == null)
                    dbObj.CreatedDate = crDate;

                dbObj.UpdatedDate = DateTime.Now;

                #region SAVE VOYAGECOSTDETAIL LIST
                if (model.VoyageCostDetails == null)
                    model.VoyageCostDetails = new VoyageCostDetailModel[0];

                var toBeRemovedDetail = dbObj.VoyageCostDetail
                    .Where(d => !model.VoyageCostDetails.Where(m => m.NewDetail == false)
                        .Select(m => m.Id).ToArray().Contains(d.Id)
                    ).ToArray();
                foreach (var item in toBeRemovedDetail)
                {
                    repoDetail.Delete(item);
                }

                foreach (var item in model.VoyageCostDetails)
                {
                    if (item.NewDetail == true)
                    {
                        var dbDetail = new VoyageCostDetail();
                        item.MapTo(dbDetail);
                        dbDetail.VoyageCost = dbObj;
                        repoDetail.Add(dbDetail);
                        var dbDriverAccountDetail = repoDriverAccountDetail.Get(d=>d.VoyageCostDetailId == dbDetail.Id);
                        var dbDriverAccount = repoDriverAccount.Get(d => d.DriverId == dbDetail.DriverId && d.ForexTypeId == dbDetail.ForexTypeId);
                        if (dbDriverAccountDetail == null && dbDriverAccount !=null && dbDetail.PayType == (int)PayType.Cash)
                        {
                            var dbDADetail = new DriverAccountDetail();
                            if(dbDetail.CostCategoryId != null)
                                dbDADetail.CostCategoryId = (int)dbDetail.CostCategoryId;
                            //dbDADetail.ActionType = (int)ActionType.Exit;
                            dbDADetail.CreatedDate = DateTime.Now;
                            dbDADetail.CreatedUserId = userId;
                            dbDADetail.DriverId = (int)dbDetail.DriverId;
                            dbDADetail.ForexTypeId = (int)dbDetail.ForexTypeId;
                            dbDADetail.OverallTotal = (decimal)dbDetail.OverallTotal;
                            dbDADetail.VoyageId = model.VoyageId;
                            dbDADetail.DriverAccountId = dbDriverAccount.Id;
                            dbDADetail.TowingVehicleId = dbDetail.TowingVehicleId;
                            dbDADetail.VoyageCostDetailId = dbDetail.Id;
                            dbDADetail.KmHour = dbDetail.KmHour;
                            dbDADetail.Quantity = dbDetail.Quantity;
                            dbDADetail.CountryId = dbDetail.CountryId;
                            dbDADetail.UnitTypeId = dbDetail.UnitTypeId;
                            dbDADetail.ActionType = dbDetail.ActionType;
                            dbDADetail.Explanation ="Sistem tarafından otomatik olarak oluşturuldu."; 
                            repoDriverAccountDetail.Add(dbDADetail);
                            if (dbDetail.ActionType == (int)ActionType.Exit)
                                dbDriverAccount.Balance = dbDriverAccount.Balance - dbDADetail.OverallTotal;
                            if (dbDetail.ActionType == (int)ActionType.Entry)
                                dbDriverAccount.Balance = dbDriverAccount.Balance + dbDADetail.OverallTotal;

                        }
                    }
                    else if (!toBeRemovedDetail.Any(d => d.Id == item.Id))
                    {
                        var dbDetail = repoDetail.GetById(item.Id);
                        item.MapTo(dbDetail);
                        dbDetail.VoyageCostId = dbObj.Id;
                        dbDetail.VoyageCost = dbObj;
                    }
                }
                #endregion
                _unitOfWork.SaveChanges();

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

        #endregion
    }
}
