using Heka.DataAccess.Context;
using HekaMOLD.Business.Helpers;
using HekaMOLD.Business.Models.Constants;
using HekaMOLD.Business.Models.DataTransfer.Receipt;
using HekaMOLD.Business.Models.DataTransfer.Reporting;
using HekaMOLD.Business.Models.DataTransfer.Summary;
using HekaMOLD.Business.Models.DataTransfer.Warehouse;
using HekaMOLD.Business.Models.Filters;
using HekaMOLD.Business.Models.Operational;
using HekaMOLD.Business.UseCases.Core.Base;
using Microsoft.Reporting.WebForms;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    var dbObj = repo.Get(d => d.Id == objectId);
                    if (dbObj == null)
                        throw new Exception("İrsaliye kaydı bulunamadi.");

                    List<DeliverySerialListModel> data = new List<DeliverySerialListModel>();

                    foreach (var item in dbObj.ItemReceiptDetail)
                    {
                        data.Add(new DeliverySerialListModel
                        {
                            ProductCode = item.Item.ItemNo,
                            ProductName = item.Item.ItemName,
                            Quantity = item.Quantity ?? 0,
                            ReceiptDate = string.Format("{0:dd.MM.yyyy}", dbObj.ReceiptDate),
                            ReceiverText = dbObj.Firm != null ? dbObj.Firm.FirmName + "\r\n" +
                                dbObj.Firm.Address : "",
                        });
                    }

                    return data;
                }
                else if (reportType == ReportType.RawMaterialLabel)
                {
                    var repo = _unitOfWork.GetRepository<ItemReceiptDetail>();
                    var dbObj = repo.Get(d => d.Id == objectId);
                    if (dbObj == null)
                        throw new Exception("İrsaliye kaydı bulunamadi.");

                    List<ItemReceiptDetailModel> data = new List<ItemReceiptDetailModel>();
                    var modelDetail = new ItemReceiptDetailModel();
                    dbObj.MapTo(modelDetail);

                    modelDetail.ItemNo = dbObj.Item != null ? dbObj.Item.ItemNo : "";
                    modelDetail.ItemName = dbObj.Item != null ? dbObj.Item.ItemName : "";
                    modelDetail.FirmCode = dbObj.ItemReceipt.Firm != null ? dbObj.ItemReceipt.Firm.FirmCode : "";
                    modelDetail.FirmName = dbObj.ItemReceipt.Firm != null ? dbObj.ItemReceipt.Firm.FirmName : "";
                    modelDetail.QuantityStr = string.Format("{0:N2}", dbObj.Quantity ?? 0);
                    modelDetail.CreatedDateStr = string.Format("{0:dd.MM.yyyy}", DateTime.Now);

                    // GENERATE BARCODE IMAGE
                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(dbObj.Id.ToString(), QRCodeGenerator.ECCLevel.Q);
                    QRCode qrCode = new QRCode(qrCodeData);
                    System.Drawing.Bitmap qrCodeImage = qrCode.GetGraphic(100);

                    System.Drawing.ImageConverter converter = new System.Drawing.ImageConverter();
                    var imgBytes = (byte[])converter.ConvertTo(qrCodeImage, typeof(byte[]));

                    modelDetail.BarcodeImage = imgBytes;

                    data.Add(modelDetail);

                    return data;
                }
                else if (reportType == ReportType.PalletLabel)
                {
                    var repo = _unitOfWork.GetRepository<Pallet>();
                    var dbObj = repo.Get(d => d.Id == objectId);
                    if (dbObj == null)
                        throw new Exception("Palet kaydı bulunamadi.");

                    List<PalletModel> data = new List<PalletModel>();
                    var modelDetail = new PalletModel();
                    dbObj.MapTo(modelDetail);

                    modelDetail.CreatedDateStr = string.Format("{0:dd.MM.yyyy}", DateTime.Now);

                    // GENERATE BARCODE IMAGE
                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(dbObj.PalletNo, QRCodeGenerator.ECCLevel.Q);
                    QRCode qrCode = new QRCode(qrCodeData);
                    System.Drawing.Bitmap qrCodeImage = qrCode.GetGraphic(100);

                    System.Drawing.ImageConverter converter = new System.Drawing.ImageConverter();
                    var imgBytes = (byte[])converter.ConvertTo(qrCodeImage, typeof(byte[]));

                    modelDetail.BarcodeImage = imgBytes;

                    data.Add(modelDetail);

                    return data;
                }
            }
            catch (Exception)
            {

            }

            return null;
        }
        #endregion
        
        public BusinessResult PrintReport<T>(int reportId, int printerId, T dataModel)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<ReportTemplate>();
                var repoPrinter = _unitOfWork.GetRepository<SystemPrinter>();

                var dbPrinter = repoPrinter.Get(d => d.Id == printerId);
                var dbTemplate = repo.Get(d => d.Id == reportId);
                if (dbTemplate == null)
                    throw new Exception("Rapor şablonu bulunamadı.");

                result = PrintReportTemplate<T>(dataModel, dbTemplate.FileName, dbPrinter.PageWidth ?? 0, 
                    dbPrinter.PageHeight ?? 0, dbPrinter.AccessPath);

                //result.Result = true;
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
        #endregion

        #region SYSTEM REPORTS
        public ItemStateModel[] GetItemStates(int[] warehouseList)
        {
            ItemStateModel[] data = new ItemStateModel[0];

            try
            {
                var repo = _unitOfWork.GetRepository<ItemReceiptDetail>();

                var movements = repo.Filter(d => warehouseList.Contains(d.ItemReceipt.InWarehouseId ?? 0)
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
                    && d.WorkOrderDetail != null
                    && d.SignalStatus == 1)
                    .GroupBy(d => new
                    {
                        WorkOrderDetail = d.WorkOrderDetail,
                        WorkDate = DbFunctions.TruncateTime(d.ShiftBelongsToDate),
                        Machine = d.Machine,
                        Shift = d.Shift,
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
                       WorkDate = DbFunctions.TruncateTime(d.CreatedDate),
                       Shift = d.Shift,
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
        #endregion
    }
}
