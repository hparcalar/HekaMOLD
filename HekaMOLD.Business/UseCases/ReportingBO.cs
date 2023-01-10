﻿using Heka.DataAccess.Context;
using HekaMOLD.Business.Helpers;
using HekaMOLD.Business.Models.Constants;
using HekaMOLD.Business.Models.DataTransfer.Receipt;
using HekaMOLD.Business.Models.DataTransfer.Reporting;
using HekaMOLD.Business.Models.DataTransfer.Summary;
using HekaMOLD.Business.Models.Filters;
using HekaMOLD.Business.Models.Operational;
using HekaMOLD.Business.UseCases.Core.Base;
using Microsoft.Reporting.WebForms;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace HekaMOLD.Business.UseCases
{
    public class ReportingBO : CoreSystemBO
    {
        #region PREPARING REPORT DATA 
        public object PrepareReportData(int objectId, ReportType reportType)
        {
            try
            {
                if (reportType == ReportType.DeliverySerialList)
                {
                    var repo = _unitOfWork.GetRepository<ItemReceipt>();
                    var repoItem = _unitOfWork.GetRepository<Item>();
                    var repoMoldTest = _unitOfWork.GetRepository<MoldTest>();

                    var dbObj = repo.Get(d => d.Id == objectId);
                    if (dbObj == null)
                        throw new Exception("İrsaliye kaydı bulunamadi.");

                    List<DeliverySerialListModel> data = new List<DeliverySerialListModel>();

                    foreach (var item in dbObj.ItemReceiptDetail)
                    {
                        string packSize = "";
                        decimal? rawMaterialGr = null;
                        var dbItem = repoItem.Get(d => d.Id == item.ItemId);
                        if (dbItem != null)
                        {
                            var dbMoldTest = repoMoldTest.Filter(d => d.ProductCode == dbItem.ItemNo)
                                .OrderByDescending(d => d.TestDate).FirstOrDefault();
                            if (dbMoldTest != null)
                            {
                                packSize = dbMoldTest.PackageDimension;
                                rawMaterialGr = dbMoldTest.RawMaterialGr;
                            }
                        }

                        if (string.IsNullOrEmpty(packSize))
                        {
                            packSize = item.PackageDimension;
                        }

                        item.NetWeight = item.Quantity * (rawMaterialGr / 1000.0m);

                        data.Add(new DeliverySerialListModel
                        {
                            ProductCode = item.Item.ItemNo,
                            ProductName = item.Item.ItemName,
                            Quantity = item.Quantity ?? 0,
                            ReceiptDate = string.Format("{0:dd.MM.yyyy}", dbObj.ReceiptDate),
                            ReceiverText = dbObj.Firm != null ? dbObj.Firm.FirmName + "\r\n" +
                                dbObj.Firm.Address : "",
                            PackageCount = item.ItemSerial.Count(),
                            PackageSize = packSize,
                            NetWeight = item.NetWeight,
                            GrossWeight = item.GrossWeight,
                            PalletCount = item.PalletCount ?? 0,
                            Driver = dbObj.Driver,
                        });
                    }

                    _unitOfWork.SaveChanges();

                    return data;
                }
            }
            catch (Exception)
            {

            }

            return null;
        }
        #endregion
        public byte[] GenerateQRCode(string content)
        {
            try
            {
                // GENERATE BARCODE IMAGE
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);
                Bitmap qrCodeImage = qrCode.GetGraphic(100);

                ImageConverter converter = new ImageConverter();
                var imgBytes = (byte[])converter.ConvertTo(qrCodeImage, typeof(byte[]));

                return imgBytes;
            }
            catch (Exception)
            {

            }

            return null;
        }
        public BusinessResult PrintReport<T>(int reportId, int printerId, T dataModel)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<ReportTemplate>();
                var repoPrinter = _unitOfWork.GetRepository<SystemPrinter>();

                var dbPrinter = repoPrinter.Get(d => d.Id == printerId);
                var dbTemplate = repo.Get(d => d.Id == reportId);
                if (dbTemplate == null && reportId != -1)
                    throw new Exception("Rapor şablonu bulunamadı.");

                string templateName = dbTemplate != null ? dbTemplate.FileName : "ItemLabel.rdlc";

                PrintReportTemplate<T>(dataModel, templateName, dbPrinter.PageWidth ?? 0, 
                    dbPrinter.PageHeight ?? 0, dbPrinter.AccessPath);

                result.Result = true;
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public BusinessResult ExportReportAsPdf<T>(int reportId, T dataModel, string outputPath, string outputFileName)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<ReportTemplate>();

                var dbTemplate = repo.Get(d => d.Id == reportId);
                if (dbTemplate == null)
                    throw new Exception("Rapor şablonu bulunamadı.");

                PdfReportTemplate<T>(dataModel, dbTemplate.FileName, 21, 29.7m, "", outputPath, outputFileName);

                result.Result = true;
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public BusinessResult ExportReportAsExcel<T>(int reportId, T dataModel, string outputPath, string outputFileName)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<ReportTemplate>();

                var dbTemplate = repo.Get(d => d.Id == reportId);
                if (dbTemplate == null)
                    throw new Exception("Rapor şablonu bulunamadı.");

                ExcelReportTemplate<T>(dataModel, dbTemplate.FileName, 21, 29.7m, "", outputPath, outputFileName);

                result.Result = true;
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        #region GENERIC PRINTING METHOD
        protected BusinessResult PrintReportTemplate<T>(T model, string fileName, 
            decimal pageWidth, decimal pageHeight, string printerName)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                dynamic dataList = new List<T>() {
                    model
                };

                if (model.GetType().GetGenericArguments().Length > 0)
                    dataList = model;

                LocalReport report = new LocalReport();
                report.ReportPath = System.AppDomain.CurrentDomain.BaseDirectory + "ReportDesign\\" + fileName;

                report.DataSources.Add(new ReportDataSource("DS1", dataList));
                Export(report, pageWidth, pageHeight);
                Print(printerName);

                result.Result = true;
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        protected BusinessResult PdfReportTemplate<T>(T model, string fileName,
            decimal pageWidth, decimal pageHeight, string printerName, string outputPath, string outputFileName)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                dynamic dataList = new List<T>() {
                    model
                };

                if (model.GetType().GetGenericArguments().Length > 0)
                    dataList = model;

                LocalReport report = new LocalReport();
                report.ReportPath = System.AppDomain.CurrentDomain.BaseDirectory + "ReportDesign\\" + fileName;

                report.DataSources.Add(new ReportDataSource("DS1", dataList));
                ExportPdf(report, pageWidth, pageHeight, outputPath, outputFileName);

                result.Result = true;
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        protected BusinessResult ExcelReportTemplate<T>(T model, string fileName,
           decimal pageWidth, decimal pageHeight, string printerName, string outputPath, string outputFileName)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                dynamic dataList = new List<T>() {
                    model
                };

                if (model.GetType().GetGenericArguments().Length > 0)
                    dataList = model;

                LocalReport report = new LocalReport();
                report.ReportPath = System.AppDomain.CurrentDomain.BaseDirectory + "ReportDesign\\" + fileName;

                report.DataSources.Add(new ReportDataSource("DS1", dataList));
                ExportExcel(report, pageWidth, pageHeight, outputPath, outputFileName);

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

        #region SYSTEM REPORTS
        public ItemStateModel[] GetItemStates(int[] warehouseList, int? all = null)
        {
            ItemStateModel[] data = new ItemStateModel[0];

            var periodicYear = WebConfigurationManager.AppSettings["Period"];

            var startDate = DateTime.ParseExact("01.01." + periodicYear, "dd.MM.yyyy", System.Globalization.CultureInfo.GetCultureInfo("tr"));
            var endDate = DateTime.ParseExact("31.12." + periodicYear, "dd.MM.yyyy", System.Globalization.CultureInfo.GetCultureInfo("tr"));

            try
            {
                var repo = _unitOfWork.GetRepository<ItemReceiptDetail>();

                var movements = repo.Filter(d => warehouseList.Contains(d.ItemReceipt.InWarehouseId ?? 0)
                    && ((all ?? 0) == 1 || (d.ItemReceipt.ReceiptDate >= startDate && d.ItemReceipt.ReceiptDate <= endDate))
                    && d.ItemReceipt.ReceiptType < 200);
                data = movements.GroupBy(d => new { d.Item, d.ItemReceipt.Warehouse })
                    .Select(d => new ItemStateModel
                    {
                        ItemId = d.Key.Item.Id,
                        ItemNo = d.Key.Item.ItemNo,
                        ItemName = d.Key.Item.ItemName,
                        WarehouseId = d.Key.Warehouse.Id,
                        WarehouseCode = d.Key.Warehouse.WarehouseCode,
                        WarehouseName = d.Key.Warehouse.WarehouseName,
                        ItemGroupId = d.Key.Item.ItemGroupId,
                        ItemGroupCode = d.Key.Item.ItemGroup != null ? d.Key.Item.ItemGroup.ItemGroupCode : "",
                        ItemGroupName = d.Key.Item.ItemGroup != null ? d.Key.Item.ItemGroup.ItemGroupName : "",
                        InQty = d.Where(m => m.ItemReceipt.ReceiptType < 100).Sum(m => m.Quantity) ?? 0,
                        OutQty = d.Where(m => m.ItemReceipt.ReceiptType > 100).Sum(m => m.Quantity) ?? 0,
                        TotalQty = (d.Where(m => m.ItemReceipt.ReceiptType < 100).Sum(m => m.Quantity) ?? 0)
                            - (d.Where(m => m.ItemReceipt.ReceiptType > 100).Sum(m => m.Quantity) ?? 0)
                    })
                    .OrderBy(d => d.ItemGroupId)
                    .ToArray();

                data = data.Where(d => d.InQty - d.OutQty > 0).ToArray();

                foreach (var item in data)
                {
                    item.TotalQty = item.InQty - item.OutQty;
                }
            }
            catch (Exception ex)
            {

            }

            return data;
        }

        public ItemStateModel[] GetOnlySaleOrderProducts(int[] warehouseList)
        {
            ItemStateModel[] data = new ItemStateModel[0];

            try
            {
                var repo = _unitOfWork.GetRepository<ItemReceiptDetail>();

                var movements = repo.Filter(d => warehouseList.Contains(d.ItemReceipt.InWarehouseId ?? 0)
                    &&
                    (
                        (d.ItemReceipt.ReceiptType < 100 && !d.ItemSerial.Any(m => m.WorkOrderDetail.WorkOrder.FirmId == 1))
                        ||
                        (d.ItemReceipt.ReceiptType > 100 && d.ItemReceipt.ReceiptType < 200)
                    )
                    );
                data = movements.GroupBy(d => new { d.Item, d.ItemReceipt.Warehouse })
                    .Select(d => new ItemStateModel
                    {
                        ItemId = d.Key.Item.Id,
                        ItemNo = d.Key.Item.ItemNo,
                        ItemName = d.Key.Item.ItemName,
                        WarehouseId = d.Key.Warehouse.Id,
                        WarehouseCode = d.Key.Warehouse.WarehouseCode,
                        WarehouseName = d.Key.Warehouse.WarehouseName,
                        ItemGroupId = d.Key.Item.ItemGroupId,
                        ItemGroupCode = d.Key.Item.ItemGroup != null ? d.Key.Item.ItemGroup.ItemGroupCode : "",
                        ItemGroupName = d.Key.Item.ItemGroup != null ? d.Key.Item.ItemGroup.ItemGroupName : "",
                        InQty = d.Where(m => m.ItemReceipt.ReceiptType < 100).Sum(m => m.Quantity) ?? 0,
                        OutQty = d.Where(m => m.ItemReceipt.ReceiptType > 100).Sum(m => m.Quantity) ?? 0,
                        TotalQty = (d.Where(m => m.ItemReceipt.ReceiptType < 100).Sum(m => m.Quantity) ?? 0)
                            - (d.Where(m => m.ItemReceipt.ReceiptType > 100).Sum(m => m.Quantity) ?? 0)
                    })
                    .OrderBy(d => d.ItemGroupId)
                    .ToArray();

                data = data.Where(d => d.InQty - d.OutQty > 0).ToArray();

                foreach (var item in data)
                {
                    item.TotalQty = item.InQty - item.OutQty;
                }
            }
            catch (Exception ex)
            {

            }

            return data;
        }

        public ItemStateModel[] GetSelfReadyProducts(int[] warehouseList)
        {
            ItemStateModel[] data = new ItemStateModel[0];

            try
            {
                var repo = _unitOfWork.GetRepository<ItemReceiptDetail>();

                var movements = repo.Filter(d => warehouseList.Contains(d.ItemReceipt.InWarehouseId ?? 0)
                    && 
                    (
                        (d.ItemReceipt.ReceiptType < 100 && d.ItemSerial.Any(m => m.WorkOrderDetail.WorkOrder.FirmId == 1))
                        ||
                        (d.ItemReceipt.ReceiptType > 100 && d.ItemReceipt.ReceiptType < 200)
                    )
                    );
                data = movements.GroupBy(d => new { d.Item, d.ItemReceipt.Warehouse })
                    .Select(d => new ItemStateModel
                    {
                        ItemId = d.Key.Item.Id,
                        ItemNo = d.Key.Item.ItemNo,
                        ItemName = d.Key.Item.ItemName,
                        WarehouseId = d.Key.Warehouse.Id,
                        WarehouseCode = d.Key.Warehouse.WarehouseCode,
                        WarehouseName = d.Key.Warehouse.WarehouseName,
                        ItemGroupId = d.Key.Item.ItemGroupId,
                        ItemGroupCode = d.Key.Item.ItemGroup != null ? d.Key.Item.ItemGroup.ItemGroupCode : "",
                        ItemGroupName = d.Key.Item.ItemGroup != null ? d.Key.Item.ItemGroup.ItemGroupName : "",
                        InQty = d.Where(m => m.ItemReceipt.ReceiptType < 100).Sum(m => m.Quantity) ?? 0,
                        OutQty = d.Where(m => m.ItemReceipt.ReceiptType > 100).Sum(m => m.Quantity) ?? 0,
                        TotalQty = (d.Where(m => m.ItemReceipt.ReceiptType < 100).Sum(m => m.Quantity) ?? 0)
                            - (d.Where(m => m.ItemReceipt.ReceiptType > 100).Sum(m => m.Quantity) ?? 0)
                    })
                    .OrderBy(d => d.ItemGroupId)
                    .ToArray();

                data = data.Where(d => d.InQty - d.OutQty > 0).ToArray();

                foreach (var item in data)
                {
                    item.TotalQty = item.InQty - item.OutQty;
                }
            }
            catch (Exception ex)
            {

            }

            return data;
        }

        public ItemStateModel[] GetItemStates(int[] warehouseList, BasicRangeFilter filter)
        {
            ItemStateModel[] data = new ItemStateModel[0];

            try
            {
                var repo = _unitOfWork.GetRepository<ItemReceiptDetail>();

                DateTime dtStart, dtEnd;

                if (string.IsNullOrEmpty(filter.StartDate))
                    filter.StartDate = "01.01." + DateTime.Now.Year;
                if (string.IsNullOrEmpty(filter.EndDate))
                    filter.EndDate = "31.12." + DateTime.Now.Year;

                dtStart = DateTime.ParseExact(filter.StartDate + " 00:00:00", "dd.MM.yyyy HH:mm:ss",
                        System.Globalization.CultureInfo.GetCultureInfo("tr"));
                dtEnd = DateTime.ParseExact(filter.EndDate + " 23:59:59", "dd.MM.yyyy HH:mm:ss",
                        System.Globalization.CultureInfo.GetCultureInfo("tr"));

                var movements = repo.Filter(d => warehouseList.Contains(d.ItemReceipt.InWarehouseId ?? 0)
                    && d.ItemReceipt.ReceiptType < 200
                    && d.ItemReceipt.ReceiptDate >= dtStart && d.ItemReceipt.ReceiptDate <= dtEnd
                    );
                data = movements.GroupBy(d => new { d.Item, d.ItemReceipt.Warehouse })
                    .Select(d => new ItemStateModel
                    {
                        ItemId = d.Key.Item.Id,
                        ItemNo = d.Key.Item.ItemNo,
                        ItemName = d.Key.Item.ItemName,
                        WarehouseId = d.Key.Warehouse.Id,
                        WarehouseCode = d.Key.Warehouse.WarehouseCode,
                        WarehouseName = d.Key.Warehouse.WarehouseName,
                        ItemGroupId = d.Key.Item.ItemGroupId,
                        ItemGroupCode = d.Key.Item.ItemGroup != null ? d.Key.Item.ItemGroup.ItemGroupCode : "",
                        ItemGroupName = d.Key.Item.ItemGroup != null ? d.Key.Item.ItemGroup.ItemGroupName : "",
                        InQty = d.Where(m => m.ItemReceipt.ReceiptType < 100
                            && m.ItemReceipt.ReceiptDate >= dtStart && m.ItemReceipt.ReceiptDate <= dtEnd
                            ).Sum(m => m.Quantity) ?? 0,
                        OutQty = d.Where(m => m.ItemReceipt.ReceiptType > 100
                            && m.ItemReceipt.ReceiptDate >= dtStart && m.ItemReceipt.ReceiptDate <= dtEnd).Sum(m => m.Quantity) ?? 0,
                        TotalQty = (d.Where(m => m.ItemReceipt.ReceiptType < 100
                            && m.ItemReceipt.ReceiptDate >= dtStart && m.ItemReceipt.ReceiptDate <= dtEnd).Sum(m => m.Quantity) ?? 0)
                            - (d.Where(m => m.ItemReceipt.ReceiptType > 100).Sum(m => m.Quantity) ?? 0)
                    })
                    .OrderBy(d => d.ItemGroupId)
                    .ToArray();

                data = data.Where(d => d.InQty - d.OutQty > 0).ToArray();

                foreach (var item in data)
                {
                    item.TotalQty = item.InQty - item.OutQty;
                }
            }
            catch (Exception)
            {

            }

            return data;
        }

        public ItemStateModel[] GetItemStatesOnlyEntries(int[] warehouseList, BasicRangeFilter filter)
        {
            ItemStateModel[] data = new ItemStateModel[0];

            try
            {
                var repo = _unitOfWork.GetRepository<ItemReceiptDetail>();

                DateTime dtStart, dtEnd;

                if (string.IsNullOrEmpty(filter.StartDate))
                    filter.StartDate = "01.01." + DateTime.Now.Year;
                if (string.IsNullOrEmpty(filter.EndDate))
                    filter.EndDate = "31.12." + DateTime.Now.Year;

                dtStart = DateTime.ParseExact(filter.StartDate + " 00:00:00", "dd.MM.yyyy HH:mm:ss",
                        System.Globalization.CultureInfo.GetCultureInfo("tr"));
                dtEnd = DateTime.ParseExact(filter.EndDate + " 23:59:59", "dd.MM.yyyy HH:mm:ss",
                        System.Globalization.CultureInfo.GetCultureInfo("tr"));

                var movements = repo.Filter(d => warehouseList.Contains(d.ItemReceipt.InWarehouseId ?? 0)
                    && d.ItemReceipt.ReceiptType < 100
                    && d.ItemReceipt.ReceiptDate >= dtStart && d.ItemReceipt.ReceiptDate <= dtEnd
                    );
                data = movements.GroupBy(d => new { d.Item, d.ItemReceipt.Warehouse })
                    .Select(d => new ItemStateModel
                    {
                        ItemId = d.Key.Item.Id,
                        ItemNo = d.Key.Item.ItemNo,
                        ItemName = d.Key.Item.ItemName,
                        WarehouseId = d.Key.Warehouse.Id,
                        WarehouseCode = d.Key.Warehouse.WarehouseCode,
                        WarehouseName = d.Key.Warehouse.WarehouseName,
                        ItemGroupId = d.Key.Item.ItemGroupId,
                        ItemGroupCode = d.Key.Item.ItemGroup != null ? d.Key.Item.ItemGroup.ItemGroupCode : "",
                        ItemGroupName = d.Key.Item.ItemGroup != null ? d.Key.Item.ItemGroup.ItemGroupName : "",
                        InQty = d.Where(m => m.ItemReceipt.ReceiptType < 100
                            && m.ItemReceipt.ReceiptDate >= dtStart && m.ItemReceipt.ReceiptDate <= dtEnd
                            ).Sum(m => m.Quantity) ?? 0,
                        OutQty = 0,
                        TotalQty = d.Where(m => m.ItemReceipt.ReceiptType < 100
                            && m.ItemReceipt.ReceiptDate >= dtStart && m.ItemReceipt.ReceiptDate <= dtEnd
                            ).Sum(m => m.Quantity) ?? 0
                        //(d.Where(m => m.ItemReceipt.ReceiptType < 100
                        //&& m.ItemReceipt.ReceiptDate >= dtStart && m.ItemReceipt.ReceiptDate <= dtEnd).Sum(m => m.Quantity) ?? 0)
                        //- (d.Where(m => m.ItemReceipt.ReceiptType > 100).Sum(m => m.Quantity) ?? 0)
                    })
                    .OrderBy(d => d.ItemGroupId)
                    .ToArray();

                data = data.Where(d => d.InQty - d.OutQty > 0).ToArray();

                foreach (var item in data)
                {
                    item.TotalQty = item.InQty - item.OutQty;
                }
            }
            catch (Exception)
            {

            }

            return data;
        }

        public ItemStateModel[] GetMachineStatesOnlyEntries(int[] warehouseList, BasicRangeFilter filter)
        {
            ItemStateModel[] data = new ItemStateModel[0];

            try
            {
                var repo = _unitOfWork.GetRepository<ItemReceiptDetail>();

                DateTime dtStart, dtEnd;

                if (string.IsNullOrEmpty(filter.StartDate))
                    filter.StartDate = "01.01." + DateTime.Now.Year;
                if (string.IsNullOrEmpty(filter.EndDate))
                    filter.EndDate = "31.12." + DateTime.Now.Year;

                dtStart = DateTime.ParseExact(filter.StartDate + " 00:00:00", "dd.MM.yyyy HH:mm:ss",
                        System.Globalization.CultureInfo.GetCultureInfo("tr"));
                dtEnd = DateTime.ParseExact(filter.EndDate + " 23:59:59", "dd.MM.yyyy HH:mm:ss",
                        System.Globalization.CultureInfo.GetCultureInfo("tr"));

                var repoSerial = _unitOfWork.GetRepository<ItemSerial>();
                var serialData = repoSerial.Filter(d =>
                    warehouseList.Contains(d.ItemReceiptDetail.ItemReceipt.InWarehouseId ?? 0)
                    && d.ItemReceiptDetail.ItemReceipt.ReceiptType < 100
                    && d.ItemReceiptDetail.ItemReceipt.ReceiptDate >= dtStart && d.ItemReceiptDetail.ItemReceipt.ReceiptDate <= dtEnd);

                data = serialData.GroupBy(d => new { Item = d.ItemReceiptDetail.Item, Machine = d.WorkOrderDetail.Machine })
                    .Select(d => new ItemStateModel
                    {
                        ItemId = d.Key.Item.Id,
                        ItemNo = d.Key.Item.ItemNo,
                        ItemName = d.Key.Item.ItemName,
                        ItemGroupId = d.Key.Item.ItemGroupId,
                        ItemGroupCode = d.Key.Item.ItemGroup != null ? d.Key.Item.ItemGroup.ItemGroupCode : "",
                        ItemGroupName = d.Key.Item.ItemGroup != null ? d.Key.Item.ItemGroup.ItemGroupName : "",
                        TotalQty = d.Sum(m => m.FirstQuantity) ?? 0,
                        MachineCode = d.Key.Machine != null ? d.Key.Machine.MachineCode : "",
                    }).ToArray();
            }
            catch (Exception)
            {

            }

            return data;
        }
        public ProductionHistoryModel[] GetProductionHistory(BasicRangeFilter filter)
        {
            ProductionHistoryModel[] data = new ProductionHistoryModel[0];

            try
            {
                DateTime dtStart, dtEnd;
                
                if (string.IsNullOrEmpty(filter.StartDate))
                    filter.StartDate = "01.01." + DateTime.Now.Year;
                if (string.IsNullOrEmpty(filter.EndDate))
                    filter.EndDate = "31.12." + DateTime.Now.Year;

                dtStart = DateTime.ParseExact(filter.StartDate + " 00:00:00", "dd.MM.yyyy HH:mm:ss",
                        System.Globalization.CultureInfo.GetCultureInfo("tr"));
                dtEnd = DateTime.ParseExact(filter.EndDate + " 23:59:59", "dd.MM.yyyy HH:mm:ss",
                        System.Globalization.CultureInfo.GetCultureInfo("tr"));

                var repoSignal = _unitOfWork.GetRepository<MachineSignal>();
                var repoSerial = _unitOfWork.GetRepository<WorkOrderSerial>();
                var repoWastage = _unitOfWork.GetRepository<ProductWastage>();

                if (repoSignal.Any(d => d.ShiftBelongsToDate >= dtStart && d.ShiftBelongsToDate <= dtEnd
                    && d.SignalStatus == 1))
                {
                    var dataList = repoSignal.Filter(d => d.ShiftBelongsToDate >= dtStart && d.ShiftBelongsToDate <= dtEnd
                    && d.WorkOrderDetail != null && d.User != null
                    && d.SignalStatus == 1)
                    .GroupBy(d => new
                    {
                        WorkOrderDetail = d.WorkOrderDetail,
                        WorkDate = d.ShiftBelongsToDate,
                        Machine = d.Machine,
                        Shift = d.Shift,
                        User = d.User != null ? d.User.UserName : "",
                    })
                    .Select(d => new ProductionHistoryModel
                    {
                        WorkDate = d.Key.WorkDate,
                        //WorkDateStr = string.Format("{0:dd.MM.yyyy}", d.Key.WorkDate),
                        WorkOrderDetailId = d.Key.WorkOrderDetail != null ? d.Key.WorkOrderDetail.Id : 0,
                        MachineId =  d.Key.Machine.Id,
                        MachineCode = d.Key.Machine.MachineCode,
                        MachineName = d.Key.Machine.MachineName,
                        OrderQuantity = d.Key.WorkOrderDetail != null ? d.Key.WorkOrderDetail.ItemOrderDetail.Quantity ?? 0 : 0,
                        CompleteQuantity = d.Count() -
                            (d.Key.WorkOrderDetail != null ? d.Key.WorkOrderDetail.ProductWastage
                            .Where(m => m.ShiftBelongsToDate >= dtStart && m.ShiftBelongsToDate <= dtEnd && m.ShiftId == d.Key.Shift.Id)
                            .Sum(m => m.Quantity) ?? 0 : 0),
                        ProductId = d.Key.WorkOrderDetail != null ? d.Key.WorkOrderDetail.ItemId : 0,
                        ProductCode = d.Key.WorkOrderDetail != null ? d.Key.WorkOrderDetail.Item.ItemNo : "",
                        ProductName = d.Key.WorkOrderDetail != null ? d.Key.WorkOrderDetail.Item.ItemName : "",
                        SaleOrderNo = d.Key.WorkOrderDetail != null ? d.Key.WorkOrderDetail.ItemOrderDetail.ItemOrder.DocumentNo : "",
                        SerialCount = d.Count(),
                        ShiftId = d.Key.Shift.Id,
                        UserName = d.Key.User,
                        ShiftCode = d.Key.Shift.ShiftCode,
                        ShiftName = d.Key.Shift.ShiftName,
                        WastageQuantity = d.Key.WorkOrderDetail != null ? d.Key.WorkOrderDetail.ProductWastage
                            .Where(m => m.ShiftBelongsToDate >= dtStart && m.ShiftBelongsToDate <= dtEnd && m.ShiftId == d.Key.Shift.Id)
                            .Sum(m => m.Quantity) ?? 0 : 0,
                        WorkQuantity = d.Key.WorkOrderDetail != null ? d.Key.WorkOrderDetail.Quantity : 0,
                    }).ToList();
                    dataList.ForEach(d =>
                    {
                        if (d.CompleteQuantity < 0)
                            d.CompleteQuantity = 0;
                        d.WorkDateStr = string.Format("{0:dd.MM.yyyy}", d.WorkDate);
                    });

                    data = dataList.ToArray();
                }
                else
                {
                    var dataList = repoSerial.Filter(d => d.CreatedDate >= dtStart && d.CreatedDate <= dtEnd
                   && d.SerialStatus == (int)SerialStatusType.Approved)
                   .GroupBy(d => new
                   {
                       WorkOrderDetail = d.WorkOrderDetail,
                       WorkDate = d.CreatedDate,
                       Shift = d.Shift,
                       //User = d.User != null ? d.User.UserName : "",
                   })
                   .Select(d => new ProductionHistoryModel
                   {
                       WorkDate = d.Key.WorkDate,
                        //WorkDateStr = string.Format("{0:dd.MM.yyyy}", d.Key.WorkDate),
                        WorkOrderDetailId = d.Key.WorkOrderDetail.Id,
                       MachineId = d.Key.WorkOrderDetail.MachineId,
                       MachineCode = d.Key.WorkOrderDetail.Machine.MachineCode,
                       MachineName = d.Key.WorkOrderDetail.Machine.MachineName,
                       OrderQuantity = d.Key.WorkOrderDetail.ItemOrderDetail.Quantity ?? 0,
                       CompleteQuantity = d.Sum(m => m.FirstQuantity) ?? 0,
                       ProductId = d.Key.WorkOrderDetail.ItemId,
                       ProductCode = d.Key.WorkOrderDetail.Item.ItemNo,
                       ProductName = d.Key.WorkOrderDetail.Item.ItemName,
                       SaleOrderNo = d.Key.WorkOrderDetail.ItemOrderDetail.ItemOrder.DocumentNo,
                       SerialCount = d.Count(),
                       ShiftId = d.Key.Shift.Id,
                       ShiftCode = d.Key.Shift.ShiftCode,
                       ShiftName = d.Key.Shift.ShiftName,
                       WastageQuantity = d.Key.WorkOrderDetail.ProductWastage
                           .Where(m => DbFunctions.TruncateTime(m.EntryDate) == d.Key.WorkDate)
                           .Sum(m => m.Quantity) ?? 0,
                       WorkQuantity = d.Key.WorkOrderDetail.Quantity,
                   }).ToList();
                    dataList.ForEach(d =>
                    {
                        d.WorkDateStr = string.Format("{0:dd.MM.yyyy}", d.WorkDate);
                    });

                    data = dataList.ToArray();
                }
            }
            catch (Exception ex)
            {

            }

            return data;
        }

        public SalesReportModel[] GetSalesReport(BasicRangeFilter filter)
        {
            SalesReportModel[] data = new SalesReportModel[0];

            try
            {
                DateTime dtStart, dtEnd;

                if (string.IsNullOrEmpty(filter.StartDate))
                    filter.StartDate = "01.01." + DateTime.Now.Year;
                if (string.IsNullOrEmpty(filter.EndDate))
                    filter.EndDate = "31.12." + DateTime.Now.Year;

                dtStart = DateTime.ParseExact(filter.StartDate + " 00:00:00", "dd.MM.yyyy HH:mm:ss",
                        System.Globalization.CultureInfo.GetCultureInfo("tr"));
                dtEnd = DateTime.ParseExact(filter.EndDate + " 23:59:59", "dd.MM.yyyy HH:mm:ss",
                        System.Globalization.CultureInfo.GetCultureInfo("tr"));

                var repo = _unitOfWork.GetRepository<ItemReceiptDetail>();
                var repoMoldTest = _unitOfWork.GetRepository<MoldTest>();

                data = repo.Filter(d => d.ItemReceipt.ReceiptType == (int)ItemReceiptType.ItemSelling
                    && d.ItemReceipt.ReceiptDate >= dtStart && d.ItemReceipt.ReceiptDate <= dtEnd).ToList()
                    .GroupBy(d => new
                    {
                        ItemId = d.ItemId,
                        FirmId = d.ItemReceipt.FirmId,
                        ItemGroupName = d.Item != null && d.Item.ItemGroup != null ? d.Item.ItemGroup.ItemGroupName : "",
                        ItemNo = d.Item != null ? d.Item.ItemNo : "",
                        ItemName = d.Item != null ? d.Item.ItemName : "",
                        FirmName = d.ItemReceipt.Firm != null ? d.ItemReceipt.Firm.FirmName : "",
                        ConsumptionItemName = d.Item != null && repoMoldTest.Filter(m => m.ProductCode == d.Item.ItemNo).Count() > 0 ?
                            repoMoldTest.Filter(m => m.ProductCode == d.Item.ItemNo).Select(m => m.RawMaterialName).FirstOrDefault() : "",
                        RecipeWeight = d.Item != null && repoMoldTest.Filter(m => m.ProductCode == d.Item.ItemNo).Count() > 0 ?
                            (repoMoldTest.Filter(m => m.ProductCode == d.Item.ItemNo).Select(m => m.RawMaterialGr).FirstOrDefault() ?? 0) : 0,
                    }).Select(d => new SalesReportModel
                    {
                        ItemId = d.Key.ItemId ?? 0,
                        FirmId = d.Key.FirmId,
                        ItemNo = d.Key.ItemNo,
                        ItemGroupName = d.Key.ItemGroupName,
                        FirmName = d.Key.FirmName,
                        ItemName = d.Key.ItemName,
                        Quantity = d.Sum(m => m.Quantity),
                        ConsumptionItemName = d.Key.ConsumptionItemName,
                        ConsumptionWeight = d.Sum(m => m.Quantity) * d.Key.RecipeWeight / 1000.0m,
                    }).ToArray();
            }
            catch (Exception ex)
            {

            }

            return data;
        }

        public ItemSerialModel[] GetCurrentSerials(int warehosueId, int itemId)
        {
            ItemSerialModel[] data = new ItemSerialModel[0];

            try
            {
                var repo = _unitOfWork.GetRepository<ItemSerial>();
                var repoItemReceipt = _unitOfWork.GetRepository<ItemReceiptDetail>();

                var allMovements = repoItemReceipt.Filter(d => d.ItemId == itemId
                    && d.ItemReceipt.InWarehouseId == warehosueId);
                var totalRemaining = (allMovements.Where(d => d.ItemReceipt.ReceiptType < 100).Sum(d => d.Quantity) ?? 0)
                    - (allMovements.Where(d => d.ItemReceipt.ReceiptType > 100).Sum(d => d.Quantity) ?? 0);

                if (totalRemaining > 0)
                {
                    var currentPackages = repo.Filter(d => d.ItemReceiptDetail.ItemId == itemId
                       && d.ItemReceiptDetail.ItemReceipt.InWarehouseId == warehosueId && d.SerialStatus != (int)SerialStatusType.Used
                        && d.SerialStatus != (int)SerialStatusType.Scrap)
                       .OrderByDescending(d => d.Id)
                       .ToArray();

                    List<ItemSerialModel> pickedPackages = new List<ItemSerialModel>();

                    int packageIndex = currentPackages.Length - 1;
                    while (totalRemaining > 0)
                    {
                        if (packageIndex < 0)
                            break;

                        var pack = currentPackages[packageIndex];

                        ItemSerialModel containerPack = new ItemSerialModel();
                        pack.MapTo(containerPack);
                        containerPack.CreatedDateStr = string.Format("{0:dd.MM.yyyy}", pack.ItemReceiptDetail.ItemReceipt.ReceiptDate);
                        containerPack.MachineCode = pack.WorkOrderDetail != null &&
                            pack.WorkOrderDetail.Machine != null ? pack.WorkOrderDetail.Machine.MachineCode : "";

                        pickedPackages.Add(containerPack);

                        totalRemaining -= pack.FirstQuantity ?? 0;
                        packageIndex--;
                    }

                    data = pickedPackages.ToArray();
                }
            }
            catch (Exception)
            {

            }

            return data;
        }

        public ItemSerialModel GetSerialByBarcode(string serialNo)
        {
            ItemSerialModel data = new ItemSerialModel();

            try
            {
                var repo = _unitOfWork.GetRepository<ItemSerial>();
                var repoItemReceipt = _unitOfWork.GetRepository<ItemReceiptDetail>();
                var repoWorkSerial = _unitOfWork.GetRepository<WorkOrderSerial>();

                var dbPack = repo.Filter(d => 
                    d.SerialStatus != (int)SerialStatusType.Scrap
                    && d.SerialNo == serialNo)
                       .FirstOrDefault();

                if (dbPack != null && dbPack.SerialStatus == (int)SerialStatusType.Used)
                {
                    data.ErrorMessage = "Bu barkod daha önce okutulup sevk edilmiş";
                    return data;
                }

                //if (dbPack == null)
                //{
                //    var dbSerial = repoWorkSerial.Get(d => d.SerialNo == serialNo);
                //    if (dbSerial != null)
                //    {

                //    }
                //}

                ItemSerialModel containerPack = new ItemSerialModel();
                dbPack.MapTo(containerPack);
                try
                {
                    containerPack.CreatedDateStr = string.Format("{0:dd.MM.yyyy}", dbPack.ItemReceiptDetail.ItemReceipt.ReceiptDate);
                }
                catch (Exception)
                {

                }

                try
                {
                    containerPack.MachineCode = dbPack.WorkOrderDetail != null &&
                   dbPack.WorkOrderDetail.Machine != null ? dbPack.WorkOrderDetail.Machine.MachineCode : "";
                    containerPack.ItemId = dbPack.ItemReceiptDetail.ItemId;
                    containerPack.ItemNo = dbPack.WorkOrderDetail != null &&
                        dbPack.WorkOrderDetail.Item != null ? dbPack.WorkOrderDetail.Item.ItemNo : "";
                    containerPack.ItemName = dbPack.WorkOrderDetail != null &&
                        dbPack.WorkOrderDetail.Item != null ? dbPack.WorkOrderDetail.Item.ItemName : "";
                    containerPack.FirmName = dbPack.WorkOrderDetail != null &&
                        dbPack.WorkOrderDetail.WorkOrder.Firm != null ? dbPack.WorkOrderDetail.WorkOrder.Firm.FirmName : "";
                }
                catch (Exception)
                {

                }

                try
                {
                    if (string.IsNullOrEmpty(containerPack.ItemNo))
                    {
                        containerPack.ItemNo = dbPack.ItemReceiptDetail != null ? dbPack.ItemReceiptDetail.Item.ItemNo : "";
                        containerPack.ItemName = dbPack.ItemReceiptDetail != null ? dbPack.ItemReceiptDetail.Item.ItemName : "";
                    }

                    if (string.IsNullOrEmpty(containerPack.FirmName))
                    {
                        if (dbPack.ItemReceiptDetail != null && dbPack.ItemReceiptDetail.ItemReceipt.Firm != null)
                            containerPack.FirmName = dbPack.ItemReceiptDetail.ItemReceipt.Firm.FirmName;
                    }
                }
                catch (Exception)
                {

                }

                

                data = containerPack;
            }
            catch (Exception)
            {

            }

            return data;
        }
        public ItemStateModel[] GetConsumedRecipeData(ItemStateModel[] products)
        {
            products = products.GroupBy(d => new { ItemId = d.ItemId, Quantity = d.TotalQty })
                .Select(d => new ItemStateModel
                {
                    ItemId = d.Key.ItemId,
                    TotalQty = d.Sum(m => m.TotalQty),
                }).ToArray();

            ItemStateModel[] data = new ItemStateModel[0];

            var repoRecipe = _unitOfWork.GetRepository<ProductRecipeDetail>();

            List<ItemStateModel> tmpData = new List<ItemStateModel>();
            foreach (var prd in products)
            {
                var items = repoRecipe.Filter(d => d.ProductRecipe.ProductId == prd.ItemId).ToArray();
                foreach (var item in items)
                {
                    tmpData.Add(new ItemStateModel
                    {
                        ItemId = item.ItemId ?? 0,
                        ItemNo = item.Item.ItemNo,
                        ItemName = item.Item.ItemName,
                        TotalQty = item.Quantity * prd.TotalQty,
                    });
                }
            }

            data = tmpData.GroupBy(d => new { d.ItemId, d.ItemNo, d.ItemName })
                .Select(d => new ItemStateModel
                {
                    ItemId = d.Key.ItemId,
                    ItemNo = d.Key.ItemNo,
                    ItemName = d.Key.ItemName,
                    TotalQty = d.Sum(m => m.TotalQty) ?? 0,
                }).ToArray();

            return data;
        }
        #endregion
    }
}
