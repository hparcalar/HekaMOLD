using HekaMOLD.Business.Models.Constants;
using HekaMOLD.Business.Models.DataTransfer.Reporting;
using HekaMOLD.Business.Models.Operational;
using HekaMOLD.Business.Models.Virtual;
using HekaMOLD.Business.UseCases;
using HekaMOLD.Enterprise.Controllers.Attributes;
using HekaMOLD.Enterprise.Controllers.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HekaMOLD.Enterprise.Controllers
{
    [UserAuthFilter]
    public class PrintingController : Controller
    {
        [HttpPost]
        [FreeAction]
        public JsonResult ExportAsPdf(int objectId, int reportId, int reportType)
        {
            string outputFile = Session.SessionID + ".pdf";

            using (ReportingBO bObj = new ReportingBO())
            {
                var reportData = bObj.PrepareReportData(objectId, (ReportType)reportType);
                bObj.ExportReportAsPdf(reportId, reportData, Server.MapPath("~/Outputs") + "/",
                    Session.SessionID + ".pdf");
            }

            return Json(new { Status = 1, Path = outputFile });
        }

        [HttpPost]
        [FreeAction]
        public JsonResult AddToPrintQueue(int objectId, int reportId, int printerId, int recordType)
        {
            BusinessResult result = new BusinessResult();

            using (ReportingBO bObj = new ReportingBO())
            {
                result = bObj.AddToPrintQueue(new Business.Models.DataTransfer.Core.PrinterQueueModel
                {
                    AllocatedPrintData = Newtonsoft.Json.JsonConvert.SerializeObject(new AllocatedPrintDataModel
                    {
                        ReportTemplateId = reportId,
                    }),
                    RecordType = recordType,
                    CreatedDate = DateTime.Now,
                    PrinterId = printerId,
                    RecordId = objectId,
                });
            }

            return Json(new { Status = result.Result ? 1 : 0, ErrorMessage=result.ErrorMessage });
        }
    }
}