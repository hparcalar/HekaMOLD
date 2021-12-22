using Heka.DataAccess.Context;
using HekaMOLD.Business.Base;
using HekaMOLD.Business.Helpers;
using HekaMOLD.Business.Models.Constants;
using HekaMOLD.Business.Models.DataTransfer.Core;
using HekaMOLD.Business.Models.Operational;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.UseCases.Core.Base
{
    public class CoreSystemBO : IBusinessObject
    {
        #region PRINTER DEFINITION BUSINESS
        public SystemPrinterModel[] GetPrinterList()
        {
            List<SystemPrinterModel> data = new List<SystemPrinterModel>();

            var repo = _unitOfWork.GetRepository<SystemPrinter>();

            repo.GetAll().ToList().ForEach(d =>
            {
                SystemPrinterModel containerObj = new SystemPrinterModel();
                d.MapTo(containerObj);
                data.Add(containerObj);
            });

            return data.ToArray();
        }

        public BusinessResult SaveOrUpdatePrinter(SystemPrinterModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                if (string.IsNullOrEmpty(model.PrinterCode))
                    throw new Exception("Yazıcı kodu girilmelidir.");

                var repo = _unitOfWork.GetRepository<SystemPrinter>();

                if (repo.Any(d => (d.PrinterCode == model.PrinterCode && d.PlantId == model.PlantId)
                    && d.Id != model.Id))
                    throw new Exception("Aynı koda sahip başka bir yazıcı mevcuttur. Lütfen farklı bir kod giriniz.");

                var dbObj = repo.Get(d => d.Id == model.Id);
                if (dbObj == null)
                {
                    dbObj = new SystemPrinter();
                    repo.Add(dbObj);
                }

                model.MapTo(dbObj);

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

        public BusinessResult DeletePrinter(int id)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<SystemPrinter>();

                var dbObj = repo.Get(d => d.Id == id);
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

        public SystemPrinterModel GetPrinter(int id)
        {
            SystemPrinterModel model = new SystemPrinterModel { };

            var repo = _unitOfWork.GetRepository<SystemPrinter>();
            var dbObj = repo.Get(d => d.Id == id);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);
            }

            return model;
        }
        #endregion

        #region ALLOCATED CODE MANAGEMENT FOR SYSTEM GENERATED CODES
        public BusinessResult AllocateCode(int recordType)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<AllocatedCode>();
                string lastAllocated = repo.Filter(d => d.ObjectType == recordType)
                    .OrderByDescending(d => d.AllocatedCode1)
                    .Select(d => d.AllocatedCode1)
                    .FirstOrDefault();

                if (recordType == (int)RecordType.SerialItem)
                {
                    if (string.IsNullOrEmpty(lastAllocated))
                    {
                        var repoSerial = _unitOfWork.GetRepository<WorkOrderSerial>();
                        string lastSerialNo = repoSerial.GetAll()
                            .OrderByDescending(d => d.SerialNo)
                            .Select(d => d.SerialNo)
                            .FirstOrDefault();

                        if (string.IsNullOrEmpty(lastSerialNo))
                            lastSerialNo = "0";

                        string lastCode = string.Format("{0:00000000}", Convert.ToInt32(lastSerialNo) + 1);
                        repo.Add(new AllocatedCode
                        {
                            ObjectType = recordType,
                            CreatedDate = DateTime.Now,
                            AllocatedCode1 = lastCode,
                        });
                        result.Code = lastCode;
                    }
                    else
                    {
                        string lastCode = string.Format("{0:00000000}", Convert.ToInt32(lastAllocated) + 1);
                        repo.Add(new AllocatedCode
                        {
                            ObjectType = recordType,
                            CreatedDate = DateTime.Now,
                            AllocatedCode1 = lastCode,
                        });
                        result.Code = lastCode;
                    }
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

        #region PRINTING QUEUE BUSINESS
        public BusinessResult AddToPrintQueue(PrinterQueueModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<PrinterQueue>();
                var dbObj = new PrinterQueue
                {
                    CreatedDate = DateTime.Now,
                    OrderNo = null,
                    RecordId = model.RecordId,
                    PrinterId = model.PrinterId,
                    RecordType = model.RecordType,
                    AllocatedPrintData = model.AllocatedPrintData,
                };
                repo.Add(dbObj);

                _unitOfWork.SaveChanges();
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

        public PrinterQueueModel GetNextFromPrinterQueue(int printerId)
        {
            var repo = _unitOfWork.GetRepository<PrinterQueue>();
            return repo.Filter(d => d.PrinterId == printerId)
                .OrderBy(d => d.CreatedDate)
                .Select(d => new PrinterQueueModel
                {
                    Id = d.Id,
                    CreatedDate = d.CreatedDate,
                    OrderNo = d.OrderNo,
                    PrinterId = d.PrinterId,
                    RecordId = d.RecordId,
                    AllocatedPrintData = d.AllocatedPrintData,
                    RecordType = d.RecordType,
                }).FirstOrDefault();
        }

        public BusinessResult SetElementAsPrinted(int printerQueueId)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<PrinterQueue>();
                var dbObj = repo.Get(d => d.Id == printerQueueId);
                if (dbObj != null)
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
        #endregion

        #region NATIVE PRINTING FUNCTIONS
        protected int m_currentPageIndex;
        protected IList<Stream> m_streams;

        protected Stream CreateStream(string name,
            string fileNameExtension, Encoding encoding,
            string mimeType, bool willSeek)
        {
            Stream stream = new MemoryStream();
            m_streams.Add(stream);
            return stream;
        }

        // Export the given report as an EMF (Enhanced Metafile) file.
        protected void Export(LocalReport report, decimal pageWidth, decimal pageHeight)
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

        protected void ExportPdf(LocalReport report, string outputPath, string outputFileName)
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
            .Replace("{PageWidth}", "21cm")
            .Replace("{PageHeight}", "29.7cm")
            .Replace("{MarginTop}", "0.2cm")
            .Replace("{MarginLeft}", "0.0cm")
            .Replace("{MarginRight}", "0.0cm")
            .Replace("{MarginBottom}", "0.0cm");

            Warning[] warnings;
            //m_streams = new List<Stream>();
            //report.Render("PDF", deviceInfo, CreateStream,
            //   out warnings);
            //foreach (Stream stream in m_streams)
            //    stream.Position = 0;

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

        protected void ExportPdf(LocalReport report, string pageWidth, string pageHeight, string outputPath, string outputFileName)
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
            .Replace("{PageWidth}", pageWidth)
            .Replace("{PageHeight}", pageHeight)
            .Replace("{MarginTop}", "0.2cm")
            .Replace("{MarginLeft}", "0.0cm")
            .Replace("{MarginRight}", "0.0cm")
            .Replace("{MarginBottom}", "0.0cm");

            Warning[] warnings;
            //m_streams = new List<Stream>();
            //report.Render("PDF", deviceInfo, CreateStream,
            //   out warnings);
            //foreach (Stream stream in m_streams)
            //    stream.Position = 0;

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

        protected void ExportPdf(LocalReport report, decimal pageWidth, decimal pageHeight,
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
        protected void PrintPage(object sender, PrintPageEventArgs ev)
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

        protected void Print(string printerName)
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
    }
}
