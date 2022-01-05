using HekaMOLD.Business.Models.Constants;
using HekaMOLD.Business.Models.DataTransfer.Core;
using HekaMOLD.Business.Models.Operational;
using HekaMOLD.Business.UseCases;
using HekaMOLD.Business.UseCases.Integrations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HekaMOLD.IntegrationService
{
    public partial class frmMain : Form
    {
        Task _taskInt;
        bool _flagInt=false;
        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            StartIntegration();
        }

        private void AddLog(string message)
        {
            try
            {
                this.BeginInvoke((Action)delegate
                {
                    if (lstBox.Items.Count > 100)
                    {
                        lstBox.Items.Clear();
                    }

                    lstBox.Items.Add(string.Format("[{0:HH:mm}]", DateTime.Now) + " " + message);
                });
            }
            catch (Exception)
            {

            }
        }

        #region INTEGRATION TASK STARTERS
        private void StartIntegration()
        {
            _flagInt = true;
            _taskInt = Task.Run(UpdateIntegrations);
        }

        private void StopIntegration()
        {
            _flagInt = false;
            try
            {
                if (_taskInt != null)
                    _taskInt.Dispose();
            }
            catch (Exception)
            {

            }
        }
        #endregion

        #region INTEGRATION TASKS
        private async Task UpdateIntegrations()
        {
            while (_flagInt)
            {
                try
                {
                    using (DefinitionsBO bObj = new DefinitionsBO())
                    {
                        var syncList = bObj.GetSyncPointList()
                            //.Where(d => d.SyncPointType == (int)SyncPointType.MikroWorkData)
                            .ToArray();

                        foreach (var sync in syncList)
                        {
                            IntegratorBase entObj = null;
                            if (sync.SyncPointType == (int)SyncPointType.MikroMaster ||
                                sync.SyncPointType == (int)SyncPointType.MikroWorkData)
                                entObj = new MikroIntegrator();
                            else if (sync.SyncPointType == (int)SyncPointType.WebTicari)
                                entObj = new WebTicariIntegrator();

                            entObj.OnTransferError += EntObj_OnTransferError;                  

                            BusinessResult result = new BusinessResult();

                            if ((entObj is MikroIntegrator && sync.SyncPointType == (int)SyncPointType.MikroWorkData)
                                || !(entObj is MikroIntegrator))
                            {
                                result = entObj.PullFirms(sync);
                                AddLog(result.Result ? "Firmalar transfer edildi." : "Firma Transferi Hata: " + result.ErrorMessage);
                            }

                            if (sync.EnabledOnSalesOrders == true)
                            {
                                result = entObj.PushSaleOrders(sync);
                                AddLog(result.Result ? "Satış siparişleri (Uygulamaya) transfer edildi." : "Satış Siparişi (Uygulamaya) Transferi Hata: "
                                    + result.ErrorMessage);
                            }

                            if (sync.EnabeldOnPurchaseItemReceipts == true && entObj is MikroIntegrator)
                            {
                                var pResult = ((MikroIntegrator)entObj).PushPurchasedItems(sync);
                                AddLog(pResult.Result ? "Satınalma irsaliyeleri transfer edildi." : "Satınalma İrsaliye Transferi Hata: " + pResult.ErrorMessage);
                            }

                            if ((entObj is MikroIntegrator && sync.SyncPointType == (int)SyncPointType.MikroMaster)
                                || !(entObj is MikroIntegrator))
                            {
                                result = entObj.PullUnits(sync);
                                AddLog(result.Result ? "Birimler transfer edildi." : "Birim Transferi Hata: " + result.ErrorMessage);
                            }

                            if ((entObj is MikroIntegrator && sync.SyncPointType == (int)SyncPointType.MikroWorkData)
                                || !(entObj is MikroIntegrator))
                            {
                                result = entObj.PullItems(sync);
                                AddLog(result.Result ? "Stoklar transfer edildi." : "Stok Transferi Hata: " + result.ErrorMessage);
                            }

                            

                            if ((entObj is MikroIntegrator && sync.SyncPointType == (int)SyncPointType.MikroWorkData)
                                || !(entObj is MikroIntegrator))
                            {
                                result = entObj.PullSaleOrders(sync);
                                AddLog(result.Result ? "Satış siparişleri (Hekaya) transfer edildi." : "Satış Siparişi (Hekaya) Transferi Hata: "
                                    + result.ErrorMessage);
                            }

                            if ((entObj is MikroIntegrator && sync.SyncPointType == (int)SyncPointType.MikroWorkData)
                                || !(entObj is MikroIntegrator))
                            {
                                result = entObj.PullProductDeliveries(sync);
                                AddLog(result.Result ? "Satış irsaliyeleri (Hekaya) transfer edildi." : "Satış İrsaliyesi (Hekaya) Transferi Hata: "
                                    + result.ErrorMessage);
                            }

                            if (sync.EnabledOnConsumptionReceipts == true && entObj is MikroIntegrator)
                            {
                                var pResult = ((MikroIntegrator)entObj).PushProductions(sync);
                                AddLog(pResult.Result ? "Üretimler transfer edildi." : "Üretim Transferi Hata: " + pResult.ErrorMessage);
                            }

                            if ((entObj is MikroIntegrator && sync.SyncPointType == (int)SyncPointType.MikroWorkData)
                                || !(entObj is MikroIntegrator))
                            {
                                result = entObj.PullRecipes(sync);
                                AddLog(result.Result ? "Reçeteler transfer edildi." : "Reçete Transferi Hata: " + result.ErrorMessage);
                            }
                        }
                    }
                }
                catch (Exception)
                {

                }

                await Task.Delay(1000 * 60 * 10);
            }
        }

        private void EntObj_OnTransferError(object sender, EventArgs e)
        {
            AddLog(sender.ToString());
        }
        #endregion

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopIntegration();
        }
    }
}
