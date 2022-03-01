using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using HekaMOLD.Business.UseCases.Core.Base;
using HekaMOLD.Business.Models.Constants;
using HekaMOLD.Business.UseCases;
using HekaMOLD.Business.Models.Operational;
using HekaMOLD.Business.Models.Virtual;
using HekaMOLD.Business.Models.DataTransfer.Reporting;
using HekaMOLD.Business.Models.DataTransfer.Labels;
using HekaPrintingService.Helpers;
using HekaMOLD.Business.Models.DataTransfer.Core;
using HekaMOLD.Business.Models.DataTransfer.Production;

namespace HekaPrintingService
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        ApiHelper _api = null;

        private string[] _printerNames = new string[] { "ArgoxUretim" };
        private string _printers = string.Empty;
        protected string Printers
        {
            get
            {
                if (string.IsNullOrEmpty(_printers) || _printerNames.Length == 0)
                {
                    _printers = ConfigurationManager.AppSettings["Printers"];

                    try
                    {
                        int[] printerList = _printers.Split(',').Select(d => Convert.ToInt32(d)).ToArray();
                        List<string> pNames = new List<string>();

                        using (CoreSystemBO bObj = new CoreSystemBO())
                        {
                            foreach (var item in printerList)
                            {
                                var dbPrinter = bObj.GetPrinter(item);
                                pNames.Add(dbPrinter != null ? dbPrinter.AccessPath : "");
                            }

                            _printerNames = pNames.ToArray();
                        }
                    }
                    catch (Exception)
                    {

                    }
                    
                }

                return _printers;
            }
        }

        bool _runTask = false;
        Task _taskQueue = null;

        private void frmMain_Load(object sender, EventArgs e)
        {
            _api = new ApiHelper(ConfigurationManager.AppSettings["ApiUri"]);
            StartTask();
        }

        private void StartTask()
        {
            _runTask = true;
            _taskQueue = Task.Run(TaskLoop);
        }

        private void StopTask()
        {
            _runTask = false;

            try
            {
                if (_taskQueue != null)
                    _taskQueue.Dispose();
            }
            catch (Exception)
            {

            }
        }

        private async Task TaskLoop()
        {
            while (_runTask)
            {
                try
                {
                    int[] printerList = Printers.Split(',').Select(d => Convert.ToInt32(d)).ToArray();

                    int printerIndex = 0;
                    foreach (var printerId in printerList)
                    {
                        var queueList = await _api.GetData<PrinterQueueModel[]>("Common/GetPrintQueueList?printerId=" + printerId.ToString());
                        foreach (var queueModel in queueList)
                        {
                            if (queueModel != null && queueModel.Id > 0)
                            {
                                if (queueModel.RecordType == (int)RecordType.SerialItem)
                                {
                                    if (queueModel.RecordId == null)
                                    {
                                        if (!string.IsNullOrEmpty(queueModel.AllocatedPrintData))
                                        {
                                            var dataModel = Newtonsoft.Json
                                                .JsonConvert.DeserializeObject<AllocatedPrintDataModel>(queueModel.AllocatedPrintData);
                                            var dbWorkOrder = await _api
                                                .GetData<WorkOrderDetailModel>("Common/GetWorkOrderDetail?id=" + (dataModel.WorkOrderDetailId ?? 0));
                                            if (dbWorkOrder != null && dbWorkOrder.Id > 0)
                                            {
                                                var currentShift = await _api.GetData<ShiftModel>("Common/GetCurrentShift");

                                                using (ProductionBO prodBo = new ProductionBO())
                                                {
                                                    prodBo.PrintProductLabel(new HekaMOLD.Business.Models.DataTransfer.Labels.ProductLabel
                                                    {
                                                        BarcodeContent = dataModel.Code,
                                                        CreatedDateStr = string.Format("{0:dd.MM.yyyy}", DateTime.Now),
                                                        FirmName = dbWorkOrder.FirmName,
                                                        ShiftName = currentShift != null ? currentShift.ShiftCode : "",
                                                        InPackageQuantity = string.Format("{0:N2}", dbWorkOrder.InPackageQuantity ?? 0),
                                                        ProductCode = dbWorkOrder.ProductCode,
                                                        ProductName = dbWorkOrder.ProductName,
                                                        Weight = "",
                                                        Id = 0,
                                                    }, printerId, _printerNames[printerIndex]);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        var serialModel = await _api.GetData<WorkOrderSerialModel>("Common/GetWorkOrderSerial?id=" + queueModel.RecordId.Value);
                                        var workModel = await _api.GetData<WorkOrderDetailModel>("Common/GetWorkOrderDetail?id=" + serialModel.WorkOrderDetailId.Value);

                                        //var currentShift = await _api.GetData<ShiftModel>("Common/GetCurrentShift");
                                        //serialModel.ShiftCode = currentShift != null ? currentShift.ShiftCode : "";
                                        //serialModel.ShiftName = currentShift != null ? currentShift.ShiftCode : "";

                                        using (ProductionBO prodBo = new ProductionBO())
                                        {
                                            prodBo.PrintProductLabel(serialModel, workModel, printerId,
                                                _printerNames[printerIndex]);
                                        }
                                    }

                                    await _api.PostData<int>("Common/SetQueueAsPrinted?id=" + queueModel.Id, queueModel.Id);

                                    //AddLog(_printerNames[printerId] + " yazıcısına gönderildi.");
                                }
                                else if (queueModel.RecordType == (int)RecordType.ItemLabel)
                                {
                                    if (queueModel.RecordId != null)
                                    {
                                        ProductLabel labelData = new ProductLabel();

                                        var dbItem = await _api.GetData<ItemModel>("Common/GetItem?id=" + queueModel.RecordId.Value);

                                        if (dbItem != null && dbItem.Id > 0)
                                        {
                                            labelData.ProductCode = dbItem.ItemNo;
                                            labelData.ProductName = dbItem.ItemName;
                                            labelData.Id = dbItem.Id;
                                        }

                                        using (ReportingBO rObj = new ReportingBO())
                                        {
                                            var allocData = Newtonsoft.Json
                                                .JsonConvert.DeserializeObject<AllocatedPrintDataModel>(queueModel.AllocatedPrintData);

                                            labelData.InPackageQuantity = string.Format("{0:N2}", allocData.Quantity ?? 0);
                                            labelData.BarcodeContent = labelData.Id + "XX" + labelData.InPackageQuantity;
                                            labelData.BarcodeImage = rObj.GenerateQRCode(labelData.BarcodeContent);

                                            rObj.PrintReport<List<ProductLabel>>(-1, printerId, new List<ProductLabel>() { labelData });
                                        }
                                    }

                                    await _api.PostData<int>("Common/SetQueueAsPrinted?id=" + queueModel.Id, queueModel.Id);
                                }
                                else if (queueModel.RecordType == (int)RecordType.DeliveryList)
                                {
                                    //using (ReportingBO rObj = new ReportingBO())
                                    //{
                                    //    var allocData = Newtonsoft.Json
                                    //        .JsonConvert.DeserializeObject<AllocatedPrintDataModel>(queueModel.AllocatedPrintData);

                                    //    var reportData = (List<DeliverySerialListModel>)rObj
                                    //        .PrepareReportData(queueModel.RecordId.Value, ReportType.DeliverySerialList);
                                    //    rObj.PrintReport<List<DeliverySerialListModel>>(allocData.ReportTemplateId.Value,
                                    //        printerId, reportData);
                                    //}

                                    await _api.PostData<int>("Common/SetQueueAsPrinted?id=" + queueModel.Id, queueModel.Id);
                                }
                            }
                        }

                        #region OLD MODEL
                        //using (CoreSystemBO bObj = new CoreSystemBO())
                        //{
                        //    var queueModel = bObj.GetNextFromPrinterQueue(printerId);
                        //    if (queueModel != null && queueModel.Id > 0)
                        //    {
                        //        if (queueModel.RecordType == (int)RecordType.SerialItem)
                        //        {
                        //            BusinessResult printResult = null;
                        //            using (ProductionBO prodBo = new ProductionBO())
                        //            {
                        //                if (queueModel.RecordId == null)
                        //                {
                        //                    if (!string.IsNullOrEmpty(queueModel.AllocatedPrintData))
                        //                    {
                        //                        var dataModel = Newtonsoft.Json
                        //                            .JsonConvert.DeserializeObject<AllocatedPrintDataModel>(queueModel.AllocatedPrintData);
                        //                        var dbWorkOrder = prodBo.GetWorkOrderDetail(dataModel.WorkOrderDetailId ?? 0);
                        //                        if (dbWorkOrder != null && dbWorkOrder.Id > 0)
                        //                        {
                        //                            var currentShift = prodBo.GetCurrentShift();

                        //                            prodBo.PrintProductLabel(new HekaMOLD.Business.Models.DataTransfer.Labels.ProductLabel
                        //                            {
                        //                                BarcodeContent = dataModel.Code,
                        //                                CreatedDateStr = string.Format("{0:dd.MM.yyyy}", DateTime.Now),
                        //                                FirmName = dbWorkOrder.FirmName,
                        //                                ShiftName = currentShift != null ? currentShift.ShiftCode : "",
                        //                                InPackageQuantity = string.Format("{0:N2}", dbWorkOrder.InPackageQuantity ?? 0),
                        //                                ProductCode = dbWorkOrder.ProductCode,
                        //                                ProductName = dbWorkOrder.ProductName,
                        //                                Weight = "",
                        //                                Id = 0,
                        //                            }, printerId, _printerNames[printerIndex]);
                        //                        }
                        //                    }
                        //                }
                        //                else
                        //                    printResult = prodBo.PrintProductLabel(queueModel.RecordId.Value, printerId,
                        //                        _printerNames[printerIndex]);
                        //            }

                        //            //if (printResult.Result)
                        //            bObj.SetElementAsPrinted(queueModel.Id);

                        //            //AddLog(_printerNames[printerId] + " yazıcısına gönderildi.");
                        //        }
                        //        else if (queueModel.RecordType == (int)RecordType.ItemLabel)
                        //        {
                        //            if (queueModel.RecordId != null)
                        //            {
                        //                ProductLabel labelData = new ProductLabel();

                        //                using (DefinitionsBO dObj = new DefinitionsBO())
                        //                {
                        //                    var dbItem = dObj.GetItem(queueModel.RecordId.Value);
                        //                    if (dbItem != null && dbItem.Id > 0)
                        //                    {
                        //                        labelData.ProductCode = dbItem.ItemNo;
                        //                        labelData.ProductName = dbItem.ItemName;
                        //                        labelData.Id = dbItem.Id;
                        //                    }
                        //                }

                        //                using (ReportingBO rObj = new ReportingBO())
                        //                {
                        //                    var allocData = Newtonsoft.Json
                        //                        .JsonConvert.DeserializeObject<AllocatedPrintDataModel>(queueModel.AllocatedPrintData);

                        //                    labelData.InPackageQuantity = string.Format("{0:N2}", allocData.Quantity ?? 0);
                        //                    labelData.BarcodeContent = labelData.Id + "XX" + labelData.InPackageQuantity;
                        //                    labelData.BarcodeImage = rObj.GenerateQRCode(labelData.BarcodeContent);

                        //                    rObj.PrintReport<List<ProductLabel>>(-1, printerId, new List<ProductLabel>() { labelData });
                        //                }
                        //            }

                        //            bObj.SetElementAsPrinted(queueModel.Id);
                        //        }
                        //        else if (queueModel.RecordType == (int)RecordType.DeliveryList)
                        //        {
                        //            using (ReportingBO rObj = new ReportingBO())
                        //            {
                        //                var allocData = Newtonsoft.Json
                        //                    .JsonConvert.DeserializeObject<AllocatedPrintDataModel>(queueModel.AllocatedPrintData);

                        //                var reportData = (List<DeliverySerialListModel>)rObj
                        //                    .PrepareReportData(queueModel.RecordId.Value, ReportType.DeliverySerialList);
                        //                rObj.PrintReport<List<DeliverySerialListModel>>(allocData.ReportTemplateId.Value,
                        //                    printerId, reportData);
                        //            }

                        //            bObj.SetElementAsPrinted(queueModel.Id);
                        //        }
                        //    }
                        //}
                        #endregion

                        printerIndex++;
                    }
                }
                catch (Exception ex)
                {
                    this.BeginInvoke((Action)delegate
                    {
                        lstLog.Items.Add(ex.Message);
                    });
                }
                
                await Task.Delay(100);
            }
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopTask();
        }

        private void AddLog(string message)
        {
            this.BeginInvoke((Action)delegate
            {
                if (lstLog.Items.Count > 100)
                    lstLog.Items.Clear();

                lstLog.Items.Add(string.Format("{0:[HH:mm]}", DateTime.Now) + ": " +
                    message);
            });
        }
        private void frmMain_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                //Hide();
                notifyIconTray.Visible = true;
                notifyIconTray.ShowBalloonTip(1000);
            }
        }

        private void kapatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
