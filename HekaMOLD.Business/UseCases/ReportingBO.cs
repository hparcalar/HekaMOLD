using Heka.DataAccess.Context;
using Heka.DataAccess.Context.Models;
using HekaMOLD.Business.Models.Constants;
using HekaMOLD.Business.Models.DataTransfer.Reporting;
using HekaMOLD.Business.Models.DataTransfer.Summary;
using HekaMOLD.Business.Models.Filters;
using HekaMOLD.Business.Models.Operational;
using HekaMOLD.Business.UseCases.Core.Base;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;

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
                else if (reportType == ReportType.Cmr)
                {
                    var repo = _unitOfWork.GetRepository<ItemLoad>();
                    var dbObj = repo.Get(d => d.Id == objectId);
                    if (dbObj != null)
                    {
                        List<LoadCmrModel> data = new List<LoadCmrModel>();

                        data.Add(new LoadCmrModel
                        {
                            Id = objectId,
                            LoadCode = dbObj.LoadCode,
                            OveralWeight = Convert.ToString(dbObj.OveralWeight) + " KG",
                            ShipperFirmName = dbObj.FirmShipper != null ? dbObj.FirmShipper.FirmName:"",
                            ShipperCountry = dbObj.CountryShipper != null ? dbObj.CountryShipper.CountryName : "",
                            ShipperCity = dbObj.CityShipper != null ? dbObj.CityShipper.PostCode + " " + dbObj.CityShipper.CityName : "",
                            ShipperAddress = dbObj.ShipperFirmExplanation,
                            BuyerFirmName = dbObj.FirmBuyer != null ? dbObj.FirmBuyer.FirmName : "",
                            BuyerCountry = dbObj.CountryBuyer != null ? dbObj.CountryBuyer.CountryName : "",
                            BuyerCity = dbObj.CityBuyer != null ? dbObj.CityBuyer.PostCode + " " + dbObj.CityBuyer.CityName : "",
                            BuyerAddress = dbObj.BuyerFirmExplanation,
                            OveralQuantity = Convert.ToString(dbObj.OveralQuantity) + " KAP",
                            VehicleTraillerPlate = dbObj.Vehicle != null ? dbObj.Vehicle.Plate:""
                        });

                        return data;
                    }
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

                PrintReportTemplate<T>(dataModel, dbTemplate.FileName, dbPrinter.PageWidth ?? 0,
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

        #region REQUIRED RDLC NATIVE FUNCTIONS
        private int m_currentPageIndex;
        private IList<Stream> m_streams;

        private Stream CreateStream(string name,
            string fileNameExtension, Encoding encoding,
            string mimeType, bool willSeek)
        {
            Stream stream = new MemoryStream();
            m_streams.Add(stream);
            return stream;
        }

        // Export the given report as an EMF (Enhanced Metafile) file.
        private void Export(LocalReport report, decimal pageWidth, decimal pageHeight)
        {
            string deviceInfo =
          @"<DeviceInfo>
                <OutputFormat>EMF</OutputFormat>
                <PageWidth>{PageWidth}</PageWidth>
                <PageHeight>{PageHeight}</PageHeight>
                <MarginTop>{MarginTop}</MarginTop>
                <MarginLeft>{MarginLeft}</MarginLeft>
                <MarginRight>{MarginRight}</MarginRight>
                <MarginBottom>{MarginBottom}</MarginBottom>
             </DeviceInfo>"
            .Replace("{PageWidth}", string.Format("{0:N2}", pageWidth).Replace(",", ".") + "cm")
            .Replace("{PageHeight}", string.Format("{0:N2}", pageHeight).Replace(",", ".") + "cm")
            .Replace("{MarginTop}", "0.0cm")
            .Replace("{MarginLeft}", "0.0cm")
            .Replace("{MarginRight}", "0.0cm")
            .Replace("{MarginBottom}", "0.0cm");

            Warning[] warnings;
            m_streams = new List<Stream>();
            report.Render("Image", deviceInfo, CreateStream,
               out warnings);
            foreach (Stream stream in m_streams)
                stream.Position = 0;
        }

        private void ExportPdf(LocalReport report, decimal pageWidth, decimal pageHeight,
            string outputPath, string outputFileName)
        {
            string deviceInfo =
          @"<DeviceInfo>
                <OutputFormat>EMF</OutputFormat>
                <PageWidth>{PageWidth}</PageWidth>
                <PageHeight>{PageHeight}</PageHeight>
                <MarginTop>{MarginTop}</MarginTop>
                <MarginLeft>{MarginLeft}</MarginLeft>
                <MarginRight>{MarginRight}</MarginRight>
                <MarginBottom>{MarginBottom}</MarginBottom>
             </DeviceInfo>"
            .Replace("{PageWidth}", string.Format("{0:N2}", pageWidth).Replace(",", ".") + "cm")
            .Replace("{PageHeight}", string.Format("{0:N2}", pageHeight).Replace(",", ".") + "cm")
            .Replace("{MarginTop}", "0.2cm")
            .Replace("{MarginLeft}", "0.0cm")
            .Replace("{MarginRight}", "0.0cm")
            .Replace("{MarginBottom}", "0.0cm");

            Warning[] warnings;

            string[] streamids;
            string mimeType;
            string encoding;
            string filenameExtension;

            byte[] bytes = report.Render(
                "PDF", null, out mimeType, out encoding, out filenameExtension,
                out streamids, out warnings);

            using (FileStream fs = new FileStream(outputPath + outputFileName, FileMode.Create))
            {
                fs.Write(bytes, 0, bytes.Length);
            }
        }

        // Handler for PrintPageEvents
        private void PrintPage(object sender, PrintPageEventArgs ev)
        {
            try
            {
                Metafile pageImage = new
               Metafile(m_streams[m_currentPageIndex]);

                // Adjust rectangular area with printer margins.
                System.Drawing.Rectangle adjustedRect = new System.Drawing.Rectangle(
                    ev.PageBounds.Left - (int)ev.PageSettings.HardMarginX,
                    ev.PageBounds.Top - (int)ev.PageSettings.HardMarginY,
                    ev.PageBounds.Width,
                    ev.PageBounds.Height);

                // Draw a white background for the report
                ev.Graphics.FillRectangle(System.Drawing.Brushes.White, adjustedRect);

                // Draw the report content
                ev.Graphics.DrawImage(pageImage, adjustedRect);

                // Prepare for the next page. Make sure we haven't hit the end.
                m_currentPageIndex++;
                ev.HasMorePages = (m_currentPageIndex < m_streams.Count);
            }
            catch (Exception ex)
            {

            }
        }

        private void Print(string printerName)
        {
            if (m_streams == null || m_streams.Count == 0)
                throw new Exception("Error: no stream to print.");
            PrintDocument printDoc = new PrintDocument();
            // YAZICI ADI PARAMETRİK BELİRTİLEBİLİR
            //printDoc.PrinterSettings.PrinterName = "Microsoft Print to PDF";
            if (!printDoc.PrinterSettings.IsValid)
            {
                throw new Exception("Error: cannot find the default printer.");
            }
            else
            {
                printDoc.PrintPage += new PrintPageEventHandler(PrintPage);
                printDoc.PrinterSettings.PrinterName = printerName;
                m_currentPageIndex = 0;
                try
                {
                    printDoc.Print();
                }
                catch (Exception ex)
                {

                }

            }
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
                        InQty = d.Where(m => m.ItemReceipt.ReceiptType < 100).Sum(m => m.Quantity) ?? 0,
                        OutQty = d.Where(m => m.ItemReceipt.ReceiptType > 100).Sum(m => m.Quantity) ?? 0,
                        TotalQty = (d.Where(m => m.ItemReceipt.ReceiptType < 100).Sum(m => m.Quantity) ?? 0)
                            - (d.Where(m => m.ItemReceipt.ReceiptType > 100).Sum(m => m.Quantity) ?? 0)
                    }).ToArray();
            }
            catch (Exception)
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
                        InQty = d.Where(m => m.ItemReceipt.ReceiptType < 100
                            && m.ItemReceipt.ReceiptDate >= dtStart && m.ItemReceipt.ReceiptDate <= dtEnd
                            ).Sum(m => m.Quantity) ?? 0,
                        OutQty = d.Where(m => m.ItemReceipt.ReceiptType > 100
                            && m.ItemReceipt.ReceiptDate >= dtStart && m.ItemReceipt.ReceiptDate <= dtEnd).Sum(m => m.Quantity) ?? 0,
                        TotalQty = (d.Where(m => m.ItemReceipt.ReceiptType < 100
                            && m.ItemReceipt.ReceiptDate >= dtStart && m.ItemReceipt.ReceiptDate <= dtEnd).Sum(m => m.Quantity) ?? 0)
                            - (d.Where(m => m.ItemReceipt.ReceiptType > 100).Sum(m => m.Quantity) ?? 0)
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
                        MachineId = d.Key.Machine.Id,
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
