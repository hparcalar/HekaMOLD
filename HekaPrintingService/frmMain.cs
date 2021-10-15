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

namespace HekaPrintingService
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private string[] _printerNames = new string[0];
        private string _printers;
        protected string Printers
        {
            get
            {
                if (string.IsNullOrEmpty(_printers))
                {
                    _printers = ConfigurationManager.AppSettings["Printers"];

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

                return _printers;
            }
        }

        bool _runTask = false;
        Task _taskQueue = null;

        private void frmMain_Load(object sender, EventArgs e)
        {
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
                int[] printerList = Printers.Split(',').Select(d => Convert.ToInt32(d)).ToArray();

                int printerIndex = 0;
                foreach (var printerId in printerList)
                {
                    using (CoreSystemBO bObj = new CoreSystemBO())
                    {
                        var queueModel = bObj.GetNextFromPrinterQueue(printerId);
                        if (queueModel != null && queueModel.Id > 0)
                        {
                            if (queueModel.RecordType == (int)RecordType.SerialItem)
                            {
                                BusinessResult printResult = null;
                                using (ProductionBO prodBo = new ProductionBO())
                                {
                                    if (queueModel.RecordId == null)
                                    {
                                        if (!string.IsNullOrEmpty(queueModel.AllocatedPrintData))
                                        {
                                            var dataModel = Newtonsoft.Json
                                                .JsonConvert.DeserializeObject<AllocatedPrintDataModel>(queueModel.AllocatedPrintData);
                                            var dbWorkOrder = prodBo.GetWorkOrderDetail(dataModel.WorkOrderDetailId ?? 0);
                                            if (dbWorkOrder != null && dbWorkOrder.Id > 0)
                                            {
                                                prodBo.PrintProductLabel(new HekaMOLD.Business.Models.DataTransfer.Labels.ProductLabel
                                                {
                                                    BarcodeContent = dataModel.Code,
                                                    CreatedDateStr = string.Format("{0:dd.MM.yyyy}", DateTime.Now),
                                                    FirmName = dbWorkOrder.FirmName,
                                                    InPackageQuantity = string.Format("{0:N2}", dbWorkOrder.InPackageQuantity ?? 0),
                                                    ProductCode = dbWorkOrder.ProductCode,
                                                    ProductName = dbWorkOrder.ProductName,
                                                    Weight = "",
                                                    Id = 0,
                                                }, printerId, _printerNames[printerIndex]);
                                            }
                                        }
                                    }
                                    else
                                        printResult = prodBo.PrintProductLabel(queueModel.RecordId.Value, printerId,
                                            _printerNames[printerIndex]);
                                }

                                //if (printResult.Result)
                                bObj.SetElementAsPrinted(queueModel.Id);

                                //AddLog(_printerNames[printerId] + " yazıcısına gönderildi.");
                            }
                            else if (queueModel.RecordType == (int)RecordType.DeliveryList)
                            {
                                using (ReportingBO rObj = new ReportingBO())
                                {
                                    var allocData = Newtonsoft.Json
                                        .JsonConvert.DeserializeObject<AllocatedPrintDataModel>(queueModel.AllocatedPrintData);

                                    var reportData = (List<DeliverySerialListModel>)rObj
                                        .PrepareReportData(queueModel.RecordId.Value, ReportType.DeliverySerialList);
                                    rObj.PrintReport<List<DeliverySerialListModel>>(allocData.ReportTemplateId.Value, 
                                        printerId, reportData);
                                }

                                bObj.SetElementAsPrinted(queueModel.Id);
                            }
                        }
                    }

                    printerIndex++;
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
