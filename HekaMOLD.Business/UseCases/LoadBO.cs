﻿using Heka.DataAccess.Context;
using Heka.DataAccess.Context.Models;
using HekaMOLD.Business.Helpers;
using HekaMOLD.Business.Models.Constants;
using HekaMOLD.Business.Models.DataTransfer.Logistics;
using HekaMOLD.Business.Models.Operational;
using HekaMOLD.Business.UseCases.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using LoadCalendarModel = HekaMOLD.Business.Models.DataTransfer.Logistics.LoadCalendarModel;

namespace HekaMOLD.Business.UseCases
{
    public class LoadBO : CoreReceiptsBO
    {

        public string GetNextLoadCode(int directionId = 0)
        {
            string defaultValue = "";
            try
            {
                var repo = _unitOfWork.GetRepository<CodeCounter>();
                var dbCodeCounter = repo.Filter(d => d.CounterType == 2)
                    .OrderByDescending(d => d.Id)
                    .Select(d => d)
                    .FirstOrDefault();
                defaultValue = dbCodeCounter.FirstValue + string.Format("{0:00000}", Convert.ToInt32(directionId == 1 ? (int)dbCodeCounter.OwnExport : directionId == 2 ? (int)dbCodeCounter.OwnImport : directionId == 3 ? (int)dbCodeCounter.OwnDomestic : directionId == 4 ?
                    (int)dbCodeCounter.OwnTransit : dbCodeCounter.Id) + 1) + ((OrderTransactionDirectionType)directionId).ToCaption();
                return defaultValue;
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
            return repo.GetAll().ToList().Select(d => new ItemLoadModel
            {
                Id = d.Id,
                LoadCode = d.LoadCode,
                OrderNo = d.OrderNo,
                Billed = d.Billed == true ? d.Billed : false,
                VoyageCode = d.VoyageCode,
                LoadWeek = string.Format("{0:yyyy}-{1}", d.LoadDate ?? DateTime.Now,
                    CultureInfo
                    .InvariantCulture.Calendar.GetWeekOfYear(d.LoadDate ?? DateTime.Now, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday)),
                VoyageConverted = d.VoyageConverted == true ? d.VoyageConverted :false,
                LoadStatusType = d.LoadStatusType,
                LoadStatusTypeStr = ((LoadStatusType)d.LoadStatusType).ToCaption(),
                DischargeDateStr = string.Format("{0:dd.MM.yyyy}", d.DischargeDate),
                LoadingDateStr = string.Format("{0:dd.MM.yyyy}", d.LoadingDate),
                DateOfNeedStr = string.Format("{0:dd.MM.yyyy}", d.DateOfNeed),
                LoadOutDateStr = string.Format("{0:dd.MM.yyyy}", d.LoadOutDate),
                BringingToWarehouseDateStr = string.Format("{0:dd.MM.yyyy}", d.BringingToWarehouseDate),
                DeliveryDateStr = string.Format("{0:dd.MM.yyyy}", d.DeliveryDate),
                DeliveryFromCustomerDateStr = string.Format("{0:dd.MM.yyyy}", d.DeliveryFromCustomerDate),
                ScheduledUploadDateStr = string.Format("{0:dd.MM.yyyy}", d.ScheduledUploadDate),
                CmrCustomerDeliveryDateStr = string.Format("{0:dd.MM.yyyy}", d.CmrCustomerDeliveryDate),
                IntendedArrivalDateStr = string.Format("{0:dd.MM.yyyy}", d.IntendedArrivalDate),
                LoadExitDateStr = string.Format("{0:dd.MM.yyyy}", d.LoadExitDate),
                LoadDateStr = string.Format("{0:dd.MM.yyyy}", d.LoadDate),
                ScheduledUploadWeek = getYearAndWeekOfNumber(Convert.ToString(d.ScheduledUploadDate)),
                OrderDateStr = string.Format("{0:dd.MM.yyyy}", d.ItemOrder != null ? d.ItemOrder.OrderDate : null),
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
                ReelOwnerFirmName = d.FirmReelOwner != null ? d.FirmReelOwner.FirmName : "",
                ManufacturerFirmName = d.FirmManufacturer != null ? d.FirmManufacturer.FirmName : "",
                ShipperFirmExplanation = d.ShipperFirmExplanation,
                BuyerFirmExplanation = d.BuyerFirmExplanation,
                OveralLadametre = d.OveralLadametre,
                OveralQuantity = d.OveralQuantity,
                OveralWeight = d.OveralWeight,
                OveralVolume = d.OveralVolume,
                OverallTotal = d.OverallTotal,
                UserAuthorName = d.UserAuthor != null ? d.UserAuthor.UserName :"",
                Explanation = d.Explanation,
                ForexTypeCode = d.ForexType != null ? d.ForexType.ForexTypeCode : "",
                AgentFirmName = d.FirmAgent !=null ? d.FirmAgent.FirmName:"",


            }).ToArray();

        }
        public LoadCalendarModel[] GetLoadCalendarList()
        {
            List<LoadCalendarModel> data = new List<LoadCalendarModel>();

            var repo = _unitOfWork.GetRepository<ItemLoad>();

            repo.Filter(a=>a.LoadDate !=null).ToList().ForEach(d =>
            {
                LoadCalendarModel containerObj = new LoadCalendarModel();
                d.MapTo(containerObj);
                containerObj.text = d.FirmCustomer != null ? d.FirmCustomer.FirmName + "\r\nYük Kodu : " + d.LoadCode + "\r\nKap : " + d.OveralQuantity +"\r\nYükleme Tarihi : "+string.Format("{0:yyyy-MM-dd}", d.LoadDate)
                + "\r\nİşlem Yönü : " + ((OrderTransactionDirectionType)d.OrderTransactionDirectionType).ToCaption(): "" ;

                containerObj.startDate = string.Format("{0:yyyy-MM-dd}", d.LoadDate) + "T00:00:00.000Z"; 
                containerObj.endDate = string.Format("{0:yyyy-MM-dd}", d.LoadDate) + "T00:00:00.000Z";
                containerObj.allDay = true;
                data.Add(containerObj);
            });

            return data.ToArray();
        }
        public ItemLoadModel[] GetItemLoadExportList()
        {
            List<ItemLoadModel> data = new List<ItemLoadModel>();

            var repo = _unitOfWork.GetRepository<ItemLoad>();

            return repo.Filter(d => d.OrderTransactionDirectionType == (int)OrderTransactionDirectionType.Export).ToList().Select(d => new ItemLoadModel
            {
                Id = d.Id,
                LoadCode = d.LoadCode,
                Billed = d.Billed == true ? d.Billed : false,
                OrderNo = d.OrderNo,
                VoyageCode = d.VoyageCode,
                LoadWeek = d.LoadWeek,
                VoyageConverted = d.VoyageConverted == true ? d.VoyageConverted : false,
                LoadStatusType = d.LoadStatusType,
                LoadStatusTypeStr = ((LoadStatusType)d.LoadStatusType).ToCaption(),
                DischargeDateStr = string.Format("{0:dd.MM.yyyy}", d.DischargeDate),
                LoadingDateStr = string.Format("{0:dd.MM.yyyy}", d.LoadingDate),
                DateOfNeedStr = string.Format("{0:dd.MM.yyyy}", d.DateOfNeed),
                LoadOutDateStr = string.Format("{0:dd.MM.yyyy}", d.LoadOutDate),
                BringingToWarehouseDateStr = string.Format("{0:dd.MM.yyyy}", d.BringingToWarehouseDate),
                DeliveryDateStr = string.Format("{0:dd.MM.yyyy}", d.DeliveryDate),
                DeliveryFromCustomerDateStr = string.Format("{0:dd.MM.yyyy}", d.DeliveryFromCustomerDate),
                ScheduledUploadDateStr = string.Format("{0:dd.MM.yyyy}", d.ScheduledUploadDate),
                CmrCustomerDeliveryDateStr = string.Format("{0:dd.MM.yyyy}", d.CmrCustomerDeliveryDate),
                IntendedArrivalDateStr = string.Format("{0:dd.MM.yyyy}", d.IntendedArrivalDate),
                LoadExitDateStr = string.Format("{0:dd.MM.yyyy}", d.LoadExitDate),
                LoadDateStr = string.Format("{0:dd.MM.yyyy}", d.LoadDate),
                ScheduledUploadWeek = getYearAndWeekOfNumber(Convert.ToString(d.ScheduledUploadDate)),
                OrderDateStr = string.Format("{0:dd.MM.yyyy}", d.ItemOrder != null ? d.ItemOrder.OrderDate : null),
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
                ReelOwnerFirmName = d.FirmReelOwner != null ? d.FirmReelOwner.FirmName : "",
                ManufacturerFirmName = d.FirmManufacturer != null ? d.FirmManufacturer.FirmName : "",
                ShipperFirmExplanation = d.ShipperFirmExplanation,
                BuyerFirmExplanation = d.BuyerFirmExplanation,
                OveralLadametre = d.OveralLadametre,
                OveralQuantity = d.OveralQuantity,
                OveralWeight = d.OveralWeight,
                OveralVolume = d.OveralVolume,
                OverallTotal = d.OverallTotal,
                UserAuthorName = d.UserAuthor != null ? d.UserAuthor.UserName : "",
                Explanation = d.Explanation,
                ForexTypeCode = d.ForexType != null ? d.ForexType.ForexTypeCode : "",
                AgentFirmName = d.FirmAgent != null ? d.FirmAgent.FirmName : "",

            }).ToArray();
        }
        public ItemLoadModel[] GetItemLoadImportList()
        {
            List<ItemLoadModel> data = new List<ItemLoadModel>();

            var repo = _unitOfWork.GetRepository<ItemLoad>();

            return repo.Filter(d => d.OrderTransactionDirectionType == (int)OrderTransactionDirectionType.Import).ToList().Select(d => new ItemLoadModel
            {
                Id = d.Id,
                LoadCode = d.LoadCode,
                Billed = d.Billed == true ? d.Billed : false,
                OrderNo = d.OrderNo,
                LoadWeek = d.LoadWeek,
                VoyageConverted = d.VoyageConverted == true ? d.VoyageConverted : false,
                LoadStatusType = d.LoadStatusType,
                LoadStatusTypeStr = ((LoadStatusType)d.LoadStatusType).ToCaption(),
                DischargeDateStr = string.Format("{0:dd.MM.yyyy}", d.DischargeDate),
                LoadingDateStr = string.Format("{0:dd.MM.yyyy}", d.LoadingDate),
                DateOfNeedStr = string.Format("{0:dd.MM.yyyy}", d.DateOfNeed),
                LoadOutDateStr = string.Format("{0:dd.MM.yyyy}", d.LoadOutDate),
                BringingToWarehouseDateStr = string.Format("{0:dd.MM.yyyy}", d.BringingToWarehouseDate),
                DeliveryDateStr = string.Format("{0:dd.MM.yyyy}", d.DeliveryDate),
                DeliveryFromCustomerDateStr = string.Format("{0:dd.MM.yyyy}", d.DeliveryFromCustomerDate),
                ScheduledUploadDateStr = string.Format("{0:dd.MM.yyyy}", d.ScheduledUploadDate),
                CmrCustomerDeliveryDateStr = string.Format("{0:dd.MM.yyyy}", d.CmrCustomerDeliveryDate),
                IntendedArrivalDateStr = string.Format("{0:dd.MM.yyyy}", d.IntendedArrivalDate),
                LoadExitDateStr = string.Format("{0:dd.MM.yyyy}", d.LoadExitDate),
                LoadDateStr = string.Format("{0:dd.MM.yyyy}", d.LoadDate),
                ScheduledUploadWeek = getYearAndWeekOfNumber(Convert.ToString(d.ScheduledUploadDate)),
                OrderDateStr = string.Format("{0:dd.MM.yyyy}", d.ItemOrder != null ? d.ItemOrder.OrderDate : null),
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
                ReelOwnerFirmName = d.FirmReelOwner != null ? d.FirmReelOwner.FirmName : "",
                ManufacturerFirmName = d.FirmManufacturer != null ? d.FirmManufacturer.FirmName : "",
                ShipperFirmExplanation = d.ShipperFirmExplanation,
                BuyerFirmExplanation = d.BuyerFirmExplanation,
                OveralLadametre = d.OveralLadametre,
                OveralQuantity = d.OveralQuantity,
                OveralWeight = d.OveralWeight,
                OveralVolume = d.OveralVolume,
                OverallTotal = d.OverallTotal,
                UserAuthorName = d.UserAuthor != null ? d.UserAuthor.UserName : "",
                Explanation = d.Explanation,
                ForexTypeCode = d.ForexType != null ? d.ForexType.ForexTypeCode : "",
                AgentFirmName = d.FirmAgent != null ? d.FirmAgent.FirmName : "",
            }).ToArray();

        }
        public ItemLoadModel[] GetItemLoadDomesticList()
        {
            List<ItemLoadModel> data = new List<ItemLoadModel>();

            var repo = _unitOfWork.GetRepository<ItemLoad>();

            return repo.Filter(d => d.OrderTransactionDirectionType == (int)OrderTransactionDirectionType.Domestic).ToList().Select(d => new ItemLoadModel
            {
                Id = d.Id,
                LoadCode = d.LoadCode,
                Billed = d.Billed == true ? d.Billed : false,
                OrderNo = d.OrderNo,
                LoadWeek = d.LoadWeek,
                VoyageCode = d.VoyageCode,
                VoyageConverted = d.VoyageConverted == true ? d.VoyageConverted : false,
                LoadStatusType = d.LoadStatusType,
                LoadStatusTypeStr = ((LoadStatusType)d.LoadStatusType).ToCaption(),
                DischargeDateStr = string.Format("{0:dd.MM.yyyy}", d.DischargeDate),
                LoadingDateStr = string.Format("{0:dd.MM.yyyy}", d.LoadingDate),
                DateOfNeedStr = string.Format("{0:dd.MM.yyyy}", d.DateOfNeed),
                LoadOutDateStr = string.Format("{0:dd.MM.yyyy}", d.LoadOutDate),
                BringingToWarehouseDateStr = string.Format("{0:dd.MM.yyyy}", d.BringingToWarehouseDate),
                DeliveryDateStr = string.Format("{0:dd.MM.yyyy}", d.DeliveryDate),
                DeliveryFromCustomerDateStr = string.Format("{0:dd.MM.yyyy}", d.DeliveryFromCustomerDate),
                ScheduledUploadDateStr = string.Format("{0:dd.MM.yyyy}", d.ScheduledUploadDate),
                CmrCustomerDeliveryDateStr = string.Format("{0:dd.MM.yyyy}", d.CmrCustomerDeliveryDate),
                IntendedArrivalDateStr = string.Format("{0:dd.MM.yyyy}", d.IntendedArrivalDate),
                LoadExitDateStr = string.Format("{0:dd.MM.yyyy}", d.LoadExitDate),
                LoadDateStr = string.Format("{0:dd.MM.yyyy}", d.LoadDate),
                ScheduledUploadWeek = getYearAndWeekOfNumber(Convert.ToString(d.ScheduledUploadDate)),
                OrderDateStr = string.Format("{0:dd.MM.yyyy}", d.ItemOrder != null ? d.ItemOrder.OrderDate : null),
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
                ReelOwnerFirmName = d.FirmReelOwner != null ? d.FirmReelOwner.FirmName : "",
                ManufacturerFirmName = d.FirmManufacturer != null ? d.FirmManufacturer.FirmName : "",
                ShipperFirmExplanation = d.ShipperFirmExplanation,
                BuyerFirmExplanation = d.BuyerFirmExplanation,
                OveralLadametre = d.OveralLadametre,
                OveralQuantity = d.OveralQuantity,
                OveralWeight = d.OveralWeight,
                OveralVolume = d.OveralVolume,
                OverallTotal = d.OverallTotal,
                Explanation = d.Explanation,
                UserAuthorName = d.UserAuthor != null ? d.UserAuthor.UserName : "",
                ForexTypeCode = d.ForexType != null ? d.ForexType.ForexTypeCode : "",
                AgentFirmName = d.FirmAgent != null ? d.FirmAgent.FirmName : "",
            }).ToArray();

        }
        public ItemLoadModel[] GetItemLoadTransitList()
        {
            List<ItemLoadModel> data = new List<ItemLoadModel>();

            var repo = _unitOfWork.GetRepository<ItemLoad>();
            return repo.Filter(d => d.OrderTransactionDirectionType == (int)OrderTransactionDirectionType.Transit).ToList().Select(d => new ItemLoadModel
            {
                Id = d.Id,
                LoadCode = d.LoadCode,
                Billed = d.Billed == true ? d.Billed : false,
                OrderNo = d.OrderNo,
                LoadWeek = d.LoadWeek,
                VoyageCode = d.VoyageCode,
                VoyageConverted = d.VoyageConverted == true ? d.VoyageConverted : false,
                LoadStatusType = d.LoadStatusType,
                LoadStatusTypeStr = ((LoadStatusType)d.LoadStatusType).ToCaption(),
                DischargeDateStr = string.Format("{0:dd.MM.yyyy}", d.DischargeDate),
                LoadingDateStr = string.Format("{0:dd.MM.yyyy}", d.LoadingDate),
                DateOfNeedStr = string.Format("{0:dd.MM.yyyy}", d.DateOfNeed),
                LoadOutDateStr = string.Format("{0:dd.MM.yyyy}", d.LoadOutDate),
                BringingToWarehouseDateStr = string.Format("{0:dd.MM.yyyy}", d.BringingToWarehouseDate),
                DeliveryDateStr = string.Format("{0:dd.MM.yyyy}", d.DeliveryDate),
                DeliveryFromCustomerDateStr = string.Format("{0:dd.MM.yyyy}", d.DeliveryFromCustomerDate),
                ScheduledUploadDateStr = string.Format("{0:dd.MM.yyyy}", d.ScheduledUploadDate),
                CmrCustomerDeliveryDateStr = string.Format("{0:dd.MM.yyyy}", d.CmrCustomerDeliveryDate),
                IntendedArrivalDateStr = string.Format("{0:dd.MM.yyyy}", d.IntendedArrivalDate),
                LoadExitDateStr = string.Format("{0:dd.MM.yyyy}", d.LoadExitDate),
                LoadDateStr = string.Format("{0:dd.MM.yyyy}", d.LoadDate),
                ScheduledUploadWeek = getYearAndWeekOfNumber(Convert.ToString(d.ScheduledUploadDate)),
                OrderDateStr = string.Format("{0:dd.MM.yyyy}", d.ItemOrder != null ? d.ItemOrder.OrderDate : null),
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
                ReelOwnerFirmName = d.FirmReelOwner != null ? d.FirmReelOwner.FirmName : "",
                ManufacturerFirmName = d.FirmManufacturer != null ? d.FirmManufacturer.FirmName : "",
                ShipperFirmExplanation = d.ShipperFirmExplanation,
                BuyerFirmExplanation = d.BuyerFirmExplanation,
                OveralLadametre = d.OveralLadametre,
                OveralQuantity = d.OveralQuantity,
                OveralWeight = d.OveralWeight,
                OveralVolume = d.OveralVolume,
                OverallTotal = d.OverallTotal,
                UserAuthorName = d.UserAuthor != null ? d.UserAuthor.UserName : "",
                Explanation = d.Explanation,
                ForexTypeCode = d.ForexType != null ? d.ForexType.ForexTypeCode : "",
                AgentFirmName = d.FirmAgent != null ? d.FirmAgent.FirmName : "",
            }).ToArray();

        }
        public ItemLoadModel GetLoad(int id)
        {
            ItemLoadModel model = new ItemLoadModel { Details = new ItemLoadDetailModel[0] };

            var repo = _unitOfWork.GetRepository<ItemLoad>();
            var repoDetails = _unitOfWork.GetRepository<ItemLoadDetail>();
            var repoInvoices = _unitOfWork.GetRepository<LoadInvoice>();

            var dbObj = repo.Get(d => d.Id == id);
            if (dbObj != null) 
            {
                model = dbObj.MapTo(model);
                model.DateOfNeedStr = string.Format("{0:dd.MM.yyyy}", dbObj.DateOfNeed);
                model.OrderDateStr = string.Format("{0:dd.MM.yyyy}", dbObj.OrderDate);
                model.OrderCreatUser = dbObj.ItemOrder != null ? dbObj.ItemOrder.User != null ? dbObj.ItemOrder.User.UserName : "": "";
                model.LoadStatusTypeStr = ((LoadStatusType)dbObj.LoadStatusType).ToCaption() != null ? ((LoadStatusType)dbObj.LoadStatusType).ToCaption() : "";
                model.DischargeDateStr = string.Format("{0:dd.MM.yyyy}", dbObj.DischargeDate);
                model.LoadingDateStr = string.Format("{0:dd.MM.yyyy}", dbObj.LoadingDate);
                model.LoadOutDateStr = string.Format("{0:dd.MM.yyyy}", dbObj.LoadOutDate);
                model.ScheduledUploadDateStr = string.Format("{0:dd.MM.yyyy}", dbObj.ScheduledUploadDate);
                model.LoadDateStr = string.Format("{0:dd.MM.yyyy}", dbObj.LoadDate);
                model.BringingToWarehouseDateStr = string.Format("{0:dd.MM.yyyy}", dbObj.BringingToWarehouseDate);
                model.ReadinessDateStr = string.Format("{0:dd.MM.yyyy}", dbObj.ReadinessDate);
                model.DeliveryFromCustomerDateStr = string.Format("{0:dd.MM.yyyy}", dbObj.DeliveryFromCustomerDate);
                model.IntendedArrivalDateStr = string.Format("{0:dd.MM.yyyy}", dbObj.IntendedArrivalDate);
                model.TClosingDateStr = string.Format("{0:dd.MM.yyyy}", dbObj.TClosingDate);
                model.CmrCustomerDeliveryDateStr = string.Format("{0:dd.MM.yyyy}", dbObj.CmrCustomerDeliveryDate);
                model.LoadExitDateStr = string.Format("{0:dd.MM.yyyy}", dbObj.LoadExitDate);
                model.DeliveryDateStr = string.Format("{0:dd.MM.yyyy}", dbObj.DeliveryDate);
                model.VoyageExitDateStr = string.Format("{0:dd.MM.yyyy}", dbObj.VoyageExitDate);
                model.VoyageEndDateStr = string.Format("{0:dd.MM.yyyy}", dbObj.VoyageEndDate);
                model.KapikuleEntryDateStr = string.Format("{0:dd.MM.yyyy}", dbObj.KapikuleEntryDate);
                model.KapikuleExitDateStr = string.Format("{0:dd.MM.yyyy}", dbObj.KapikuleExitDate);
                model.InvoiceDateStr = string.Format("{0:dd.MM.yyyy}", dbObj.InvoiceDate);
                model.T1T2StartDateStr = string.Format("{0:dd.MM.yyyy}", dbObj.T1T2StartDate);
                model.OrderNo = dbObj.OrderNo;
                model.VoyageConverted = dbObj.VoyageConverted;
                model.CustomerFirmCode = dbObj.FirmCustomer != null ? dbObj.FirmCustomer.FirmCode : "";
                model.CustomerFirmName = dbObj.FirmCustomer != null ? dbObj.FirmCustomer.FirmName : "";
                model.EntryCustomsCode = dbObj.CustomsEntry != null ? dbObj.CustomsEntry.CustomsCode : "";
                model.EntryCustomsName = dbObj.CustomsEntry != null ? dbObj.CustomsEntry.CustomsName : "";
                model.ExitCustomsCode = dbObj.CustomsExit != null ? dbObj.CustomsExit.CustomsCode : "";
                model.ExitCustomsName = dbObj.CustomsExit != null ? dbObj.CustomsExit.CustomsName : "";
                //model.OrderCreatUser = dbObj.ItemOrder.User != null ? dbObj.ItemOrder.User.Login:"";
                model.OrderTransactionDirectionTypeStr = dbObj.OrderTransactionDirectionType != null ? ((OrderTransactionDirectionType)dbObj.OrderTransactionDirectionType).ToCaption() : "";
                model.OrderUploadTypeStr = dbObj.OrderUploadType == 1 ? LSabit.GET_GRUPAJ : dbObj.OrderUploadType == 2 ? LSabit.GET_COMPLATE : "";
                model.OrderUploadPointTypeStr = dbObj.OrderUploadPointType == 1 ? LSabit.GET_FROMCUSTOMER : dbObj.OrderUploadPointType == 2 ? LSabit.GET_FROMWAREHOUSE : "";
                model.OrderCalculationTypeStr = dbObj.OrderCalculationType == 1 ? LSabit.GET_WEIGHTTED : dbObj.OrderCalculationType == 2 ? LSabit.GET_VOLUMETRIC : dbObj.OrderCalculationType == 3 ? LSabit.GET_LADAMETRE : "";
                model.LoadWeek = string.Format("{0:yyyy}-{1}", dbObj.LoadDate ?? DateTime.Now,
                   CultureInfo
                   .InvariantCulture.Calendar.GetWeekOfYear(dbObj.LoadDate ?? DateTime.Now, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday));
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
                        Ladametre = d.Ladametre,
                        Stackable = d.Stackable,
                        PackageInNumber = d.PackageInNumber,
                        UnitId = d.UnitId,
                        UnitPrice = d.UnitPrice,
                        UpdatedDate = d.UpdatedDate,
                        UpdatedUserId = d.UpdatedUserId,
                        ItemCode = d.Item != null ? d.Item.ItemNo : "",
                        ItemName = d.Item != null ? d.Item.ItemName : "",
                        Explanation = d.Explanation

                    }).ToArray();

                model.Invoices = repoInvoices.Filter(d => d.ItemLoadId == dbObj.Id).ToList().Select(d => new LoadInvoiceModel {
                        Id = d.Id,
                        InvoiceType = d.InvoiceType,
                        FirmId = d.FirmId,
                        SubTotal = d.SubTotal,
                        ForexId = d.ForexId,
                        ForexRate = d.ForexRate,
                        InvoiceDateStr = string.Format("{0:dd.MM.yyy}",d.InvoiceDate),
                        FirmName = d.Firm != null ?  d.Firm.FirmName:"",
                        ForexCode = d.ForexType !=null ?  d.ForexType.ForexTypeCode:"",
                        OverallTotal = d.OverallTotal,
                        ServiceItemId = d.ServiceItemId,
                        ServiceItemName = d.ServiceItem != null ? d.ServiceItem.ServiceItemName :"",
                        TaxRate = d.TaxRate,
                        TaxAmount = d.TaxAmount,
                        TaxIncluded = d.TaxIncluded,
                        InvoiceDate = d.InvoiceDate,
                        Integration = d.Integration,
                        DocumentNo = d.DocumentNo,
                        IntegrationId = d.IntegrationId,
                        Explanation = d.Explanation
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

        public BusinessResult SaveOrUpdateLoad(ItemLoadModel model, int userId, bool detailCanBeNull = false)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<ItemLoad>();
                var repoDetail = _unitOfWork.GetRepository<ItemLoadDetail>();
                var repoOrderDetail = _unitOfWork.GetRepository<ItemOrderDetail>();
                var repoNotify = _unitOfWork.GetRepository<Notification>();
                var repoCodeCounter = _unitOfWork.GetRepository<CodeCounter>();
                var repoInvoice = _unitOfWork.GetRepository<LoadInvoice>();


                bool newRecord = false;
                var dbObj = repo.Get(d => d.Id == model.Id);
                if (dbObj == null)
                {
                    dbObj = new ItemLoad();
                    dbObj.LoadCode = GetNextLoadCode((int)model.OrderTransactionDirectionType);
                    dbObj.CreatedDate = DateTime.Now;
                    dbObj.CreatedUserId = userId;
                    dbObj.LoadStatusType = (int)LoadStatusType.Ready;
                    repo.Add(dbObj);
                    newRecord = true;
                }
                if (repo.Any(d => (d.LoadCode == model.LoadCode) && d.Id != model.Id))
                    throw new Exception("Aynı koda sahip başka bir yük mevcuttur. Lütfen farklı bir kod giriniz.");

                if (!string.IsNullOrEmpty(model.LoadDateStr))
                    model.LoadDate = DateTime.ParseExact(model.LoadDateStr, "dd.MM.yyyy",
                        System.Globalization.CultureInfo.GetCultureInfo("tr"));
                if (!string.IsNullOrEmpty(model.VoyageExitDateStr))
                    model.VoyageExitDate = DateTime.ParseExact(model.VoyageExitDateStr, "dd.MM.yyyy", System.Globalization.CultureInfo.GetCultureInfo("tr"));
                if (!string.IsNullOrEmpty(model.InvoiceDateStr))
                    model.InvoiceDate = DateTime.ParseExact(model.InvoiceDateStr, "dd.MM.yyyy", System.Globalization.CultureInfo.GetCultureInfo("tr"));
                if (!string.IsNullOrEmpty(model.VoyageEndDateStr))
                    model.VoyageEndDate = DateTime.ParseExact(model.VoyageEndDateStr, "dd.MM.yyyy", System.Globalization.CultureInfo.GetCultureInfo("tr"));
                if (!string.IsNullOrEmpty(model.KapikuleEntryDateStr))
                    model.KapikuleEntryDate = DateTime.ParseExact(model.KapikuleEntryDateStr, "dd.MM.yyyy", System.Globalization.CultureInfo.GetCultureInfo("tr"));
                if (!string.IsNullOrEmpty(model.KapikuleExitDateStr))
                    model.KapikuleExitDate = DateTime.ParseExact(model.KapikuleExitDateStr, "dd.MM.yyyy", System.Globalization.CultureInfo.GetCultureInfo("tr"));
                if (!string.IsNullOrEmpty(model.LoadDateStr))
                    model.LoadDate = DateTime.ParseExact(model.LoadDateStr, "dd.MM.yyyy", System.Globalization.CultureInfo.GetCultureInfo("tr"));
                if (!string.IsNullOrEmpty(model.LoadExitDateStr))
                    model.LoadExitDate = DateTime.ParseExact(model.LoadExitDateStr, "dd.MM.yyyy", System.Globalization.CultureInfo.GetCultureInfo("tr"));
                if (!string.IsNullOrEmpty(model.DeliveryDateStr))
                    model.DeliveryDate = DateTime.ParseExact(model.DeliveryDateStr, "dd.MM.yyyy", System.Globalization.CultureInfo.GetCultureInfo("tr"));
                if (!string.IsNullOrEmpty(model.DischargeDateStr))
                    model.DischargeDate = DateTime.ParseExact(model.DischargeDateStr, "dd.MM.yyyy", System.Globalization.CultureInfo.GetCultureInfo("tr"));
                if (!string.IsNullOrEmpty(model.DateOfNeedStr))
                    model.DateOfNeed = DateTime.ParseExact(model.DateOfNeedStr, "dd.MM.yyyy", System.Globalization.CultureInfo.GetCultureInfo("tr"));
                if (!string.IsNullOrEmpty(model.LoadOutDateStr))
                    model.LoadOutDate = DateTime.ParseExact(model.LoadOutDateStr, "dd.MM.yyyy", System.Globalization.CultureInfo.GetCultureInfo("tr"));
                if (!string.IsNullOrEmpty(model.LoadingDateStr))
                    model.LoadingDate = DateTime.ParseExact(model.LoadingDateStr, "dd.MM.yyyy", System.Globalization.CultureInfo.GetCultureInfo("tr"));
                if (!string.IsNullOrEmpty(model.TClosingDateStr))
                    model.TClosingDate = DateTime.ParseExact(model.TClosingDateStr, "dd.MM.yyyy", System.Globalization.CultureInfo.GetCultureInfo("tr"));
                if (!string.IsNullOrEmpty(model.ReadinessDateStr))
                    model.ReadinessDate = DateTime.ParseExact(model.ReadinessDateStr, "dd.MM.yyyy", System.Globalization.CultureInfo.GetCultureInfo("tr"));
                if (!string.IsNullOrEmpty(model.DeliveryFromCustomerDateStr))
                    model.DeliveryFromCustomerDate = DateTime.ParseExact(model.DeliveryFromCustomerDateStr, "dd.MM.yyyy", System.Globalization.CultureInfo.GetCultureInfo("tr"));
                if (!string.IsNullOrEmpty(model.IntendedArrivalDateStr))
                    model.IntendedArrivalDate = DateTime.ParseExact(model.IntendedArrivalDateStr, "dd.MM.yyyy", System.Globalization.CultureInfo.GetCultureInfo("tr"));
                if (!string.IsNullOrEmpty(model.CmrCustomerDeliveryDateStr))
                    model.CmrCustomerDeliveryDate = DateTime.ParseExact(model.CmrCustomerDeliveryDateStr, "dd.MM.yyyy", System.Globalization.CultureInfo.GetCultureInfo("tr"));
                if (!string.IsNullOrEmpty(model.BringingToWarehouseDateStr))
                    model.BringingToWarehouseDate = DateTime.ParseExact(model.BringingToWarehouseDateStr, "dd.MM.yyyy", System.Globalization.CultureInfo.GetCultureInfo("tr"));
                if (!string.IsNullOrEmpty(model.ScheduledUploadDateStr))
                    model.ScheduledUploadDate = DateTime.ParseExact(model.ScheduledUploadDateStr, "dd.MM.yyyy", System.Globalization.CultureInfo.GetCultureInfo("tr"));
                if (!string.IsNullOrEmpty(model.T1T2StartDateStr))
                    model.T1T2StartDate = DateTime.ParseExact(model.T1T2StartDateStr, "dd.MM.yyyy", System.Globalization.CultureInfo.GetCultureInfo("tr"));
                //else if (string.IsNullOrEmpty(model.ScheduledUploadDateStr))
                //    throw new Exception("Planlanan yükleme tarihi bilgisini giriniz !");

                if ((int)dbObj.LoadStatusType == (int)LoadStatusType.Cancelled)
                    throw new Exception("İptal edilen yükte değişiklik yapılamaz !");
                if ((int)dbObj.LoadStatusType == (int)LoadStatusType.Completed)
                    throw new Exception("Tamamlanan yükte değişiklik yapılamaz !");
                if (model.OrderTransactionDirectionType == null)
                    throw new Exception("İşlem yönü seçilmelidir !");
                if (model.LoadStatusType == null)
                    throw new Exception("Yük durumu seçiniz");

                if (model.ForexTypeId == null)
                    throw new Exception("Döviz kodu seçilmelidir !");

                if (string.IsNullOrEmpty(model.LoadCode))
                    throw new Exception("Yük kodu boş geçilemez !");

                var crDate = dbObj.CreatedDate;
                var reqStats = dbObj.LoadStatusType;
                var crUserId = dbObj.CreatedUserId;

                model.MapTo(dbObj);

                if (dbObj.CreatedDate == null)
                    dbObj.CreatedDate = crDate;
                if (dbObj.LoadStatusType == null)
                    dbObj.LoadStatusType = reqStats;
                if (dbObj.CreatedUserId == null)
                    dbObj.CreatedUserId = crUserId;
                dbObj.UpdatedDate = DateTime.Now;
                dbObj.UpdatedUserId = userId;

                #region SAVE DETAILS
                if (model.Details == null && detailCanBeNull == false)
                    throw new Exception("Detay bilgisi olmadan yük kaydedilemez.");

                foreach (var item in model.Details)
                {
                    if (item.NewDetail)
                        item.Id = 0;
                }

                if (dbObj.LoadStatusType != (int)LoadStatusType.Completed && dbObj.LoadStatusType != (int)LoadStatusType.Cancelled)
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

                        if (dbDetail.LoadStatus == null || dbDetail.LoadStatus == (int)LoadStatusType.Ready)
                            dbDetail.LoadStatus = dbObj.LoadStatusType;
                        if (dbObj.Id > 0)
                            dbDetail.ItemLoadId = dbObj.Id;

                        dbDetail.LineNumber = lineNo;

                        lineNo++;
                    }
                }
                #endregion
                #region SAVE INVOICES

                if (model.Invoices == null)
                    model.Invoices = new LoadInvoiceModel[0];

                var toBeRemovedInvoices = dbObj.LoadInvoice
                    .Where(d => !model.Invoices.Where(m => m.NewDetail == false)
                        .Select(m => m.Id).ToArray().Contains(d.Id)
                    ).ToArray();
                foreach (var item in toBeRemovedInvoices)
                {
                    repoInvoice.Delete(item);
                }

                foreach (var item in model.Invoices)
                {
                    if (item.NewDetail == true)
                    {
                        var dbItemAu = new LoadInvoice();
                        item.MapTo(dbItemAu);
                        dbItemAu.ItemLoad = dbObj;
                        repoInvoice.Add(dbItemAu);
                    }
                    else if (!toBeRemovedInvoices.Any(d => d.Id == item.Id))
                    {
                        var dbItemAu = repoInvoice.GetById(item.Id);
                        var invoiceDate = dbItemAu.InvoiceDate;
                        item.MapTo(dbItemAu);
                        if (item.InvoiceDate == null)
                        {
                            dbItemAu.InvoiceDate = invoiceDate;
                        }
                        //dbItemAu.InvoiceDate =Convert.ToDateTime( item.InvoiceDateStr);
                        dbItemAu.ItemLoadId = dbObj.Id;
                    }
                }
                #endregion
                #region CODECOUNTER
                var objCodeCounter = repoCodeCounter.Filter(d => d.CounterType == 2)
                    .OrderByDescending(d => d.Id)
                    .Select(d => d)
                    .FirstOrDefault();
                var dbrepoCodeCounter = repoCodeCounter.Get(d => d.Id == objCodeCounter.Id);
                if (newRecord)
                {
                    if (model.OrderTransactionDirectionType == 1)
                    {
                        dbrepoCodeCounter.OwnExport++;
                    }
                    if (model.OrderTransactionDirectionType == 2)
                    {
                        dbrepoCodeCounter.OwnImport++;
                    }
                    if (model.OrderTransactionDirectionType == 3)
                    {
                        dbrepoCodeCounter.OwnDomestic++;
                    }
                    if (model.OrderTransactionDirectionType == 4)
                    {
                        dbrepoCodeCounter.OwnTransit++;
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

        //public BusinessResult ApproveLoad(int id, int userId)
        //{
        //    BusinessResult result = new BusinessResult();

        //    try
        //    {
        //        var repo = _unitOfWork.GetRepository<ItemLoad>();

        //        var dbObj = repo.Get(d => d.Id == id);
        //        if (dbObj == null)
        //            throw new Exception("Onaylanması beklenen yük kaydına ulaşılamadı.");

        //        if (dbObj.LoadStatusType != (int)LoadStatusType.Created)
        //            throw new Exception("Onay bekleyen durumunda olmayan bir sipariş onaylanamaz.");

        //        dbObj.LoadStatusType = (int)LoadStatusType.Approved;
        //        dbObj.UpdatedDate = DateTime.Now;
        //        dbObj.UpdatedUserId = userId;

        //        foreach (var item in dbObj.ItemLoadDetail)
        //        {
        //            item.LoadStatus = (int)LoadStatusType.Approved;
        //        }

        //        _unitOfWork.SaveChanges();

        //        #region CREATE NOTIFICATIONS
        //        base.CreateNotification(new Models.DataTransfer.Core.NotificationModel
        //        {
        //            IsProcessed = false,
        //            Message = "Yük kodu: " + dbObj.LoadCode
        //                    + " olan yük onaylandı.",
        //            Title = NotifyType.ItemLoadIsApproved.ToCaption(),
        //            NotifyType = (int)NotifyType.ItemOrderIsApproved,
        //            SeenStatus = 0,
        //            RecordId = dbObj.Id,
        //            UserId = dbObj.CreatedUserId
        //        });
        //        #endregion

        //        result.RecordId = dbObj.Id;
        //        result.Result = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Result = false;
        //        result.ErrorMessage = ex.Message;
        //    }

        //    return result;
        //}
        public BusinessResult CancelledLoad(int id, int userId)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<ItemLoad>();
                var repoItemOrder = _unitOfWork.GetRepository<ItemOrder>();

                var dbObj = repo.Get(d => d.Id == id);
                var dbOrderObj = repoItemOrder.Get(d => d.OrderNo == dbObj.OrderNo);

                if (dbObj == null)
                    throw new Exception("Yük kaydına ulaşılamadı.");

                if (dbObj.LoadStatusType == (int)LoadStatusType.Cancelled)
                    throw new Exception("Bu yük daha önceden iptal edilmiştir.");

                if (dbObj.VoyageConverted == true)
                    throw new Exception("Sefere dönüştürülen yük iptal edilemez.");

                if (dbOrderObj != null)
                {
                    dbOrderObj.OrderStatus = (int)OrderStatusType.Approved;
                    foreach (var item in dbOrderObj.ItemOrderDetail)
                    {
                        item.OrderStatus = (int)OrderStatusType.Approved;
                    }
                }
                dbObj.LoadStatusType = (int)LoadStatusType.Cancelled;
                dbObj.UpdatedDate = DateTime.Now;
                dbObj.UpdatedUserId = userId;
                dbObj.OrderNo = "";

                foreach (var item in dbObj.ItemLoadDetail)
                {
                    item.LoadStatus = (int)LoadStatusType.Cancelled;
                }
                _unitOfWork.SaveChanges();

                #region CREATE NOTIFICATIONS
                base.CreateNotification(new Models.DataTransfer.Core.NotificationModel
                {
                    IsProcessed = false,
                    Message = string.Format("{0:dd.MM.yyyy}", dbObj.LoadDate)
                            + " tarihinde oluşturduğunuz yük iptal edildi.",
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
        public int GetNextRecord(int plantId, int Id)
        {
            try
            {
                var repo = _unitOfWork.GetRepository<ItemLoad>();
                int lastLoadNo = repo.Filter(d => d.PlantId == plantId && d.Id > Id)
                    .OrderBy(d => d.Id)
                    .Select(d => d.Id)
                    .FirstOrDefault();

                if (string.IsNullOrEmpty(Convert.ToString(lastLoadNo)))
                    lastLoadNo = 0;

                return lastLoadNo;
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
                var repo = _unitOfWork.GetRepository<ItemLoad>();
                int lastLoadNo = repo.Filter(d => d.PlantId == plantId && d.Id < Id)
                    .OrderByDescending(d => d.Id)
                    .Select(d => d.Id)
                    .FirstOrDefault();

                if (string.IsNullOrEmpty(Convert.ToString(lastLoadNo)))
                    lastLoadNo = 0;

                return lastLoadNo;
            }
            catch (Exception)
            {

            }

            return default;
        }
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
