using HekaMOLD.Business.UseCases;
using HekaMOLD.Business.Models.DataTransfer.Production;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace HekaMOLD.DataImporter
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
        }

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            if (ofDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    txtFileName.Text = ofDialog.SafeFileName;

                    int transferredCount = 0, errorCount = 0;

                    using (var package = new ExcelPackage(new FileInfo(ofDialog.FileName)))
                    {
                        foreach (var sheet in package.Workbook.Worksheets)
                        {
                            try
                            {
                                string macName = "";
                                try
                                {
                                    macName = sheet.Cells[10, 3].Value.ToString();
                                }
                                catch (Exception)
                                {

                                }
                                string productDesc = "";

                                try
                                {
                                    productDesc = sheet.Cells[11, 3].Value.ToString();
                                }
                                catch (Exception)
                                {

                                }

                                string headSize = "";

                                try
                                {
                                    headSize = sheet.Cells[12, 3].Value.ToString();
                                }
                                catch (Exception)
                                {

                                }

                                string rawMatName = "";
                                try
                                {
                                    rawMatName = sheet.Cells[13, 3].Value.ToString();
                                }
                                catch (Exception)
                                {

                                }

                                string rawMatGr = "";
                                try
                                {
                                    rawMatGr = sheet.Cells[14, 3].Value.ToString();
                                }
                                catch (Exception)
                                {

                                }

                                string inflTime = "";
                                try
                                {
                                    inflTime = sheet.Cells[15, 3].Value.ToString();
                                }
                                catch (Exception)
                                {

                                }

                                string totalTime = "";
                                try
                                {
                                    totalTime = sheet.Cells[16, 3].Value.ToString();
                                }
                                catch (Exception)
                                {

                                }

                                string dyeCode = "";
                                try
                                {
                                    dyeCode = sheet.Cells[17, 3].Value.ToString();
                                }
                                catch (Exception)
                                {

                                }

                                string ralCode = "";
                                try
                                {
                                    ralCode = sheet.Cells[17, 6].Value.ToString();
                                }
                                catch (Exception)
                                {

                                }

                                string packSize = "";
                                try
                                {
                                    packSize = sheet.Cells[18, 3].Value.ToString();
                                }
                                catch (Exception)
                                {

                                }

                                string inPckQty = "";
                                try
                                {
                                    inPckQty = sheet.Cells[19, 3].Value.ToString();
                                }
                                catch (Exception)
                                {

                                }

                                string nutQty = "";
                                try
                                {
                                    nutQty = sheet.Cells[20, 3].Value.ToString();
                                }
                                catch (Exception)
                                {

                                }

                                string nutCaliber = "";
                                try
                                {
                                    sheet.Cells[20, 6].Value.ToString();
                                }
                                catch (Exception)
                                {

                                }
                                string inPltQty = "";
                                try
                                {
                                    inPltQty = sheet.Cells[21, 3].Value.ToString();
                                }
                                catch (Exception)
                                {

                                }

                                string productCode = "";
                                try
                                {
                                    productCode = sheet.Cells[22, 3].Value.ToString();
                                }
                                catch (Exception)
                                {

                                }

                                string productName = "";
                                try
                                {
                                    productName = sheet.Cells[23, 3].Value.ToString();
                                }
                                catch (Exception)
                                {

                                }

                                string moldCode = "";
                                try
                                {
                                    moldCode = sheet.Cells[24, 3].Value.ToString();
                                }
                                catch (Exception)
                                {

                                }

                                string moldName = "";
                                try
                                {
                                    moldName = sheet.Cells[25, 3].Value.ToString();
                                }
                                catch (Exception)
                                {

                                }

                                string[] rawInfo = Regex.Split(rawMatGr, "±");

                                int machineId = 0;

                                using (DefinitionsBO bObj = new DefinitionsBO())
                                {
                                    var dbMac = bObj.GetMachine(Regex.Match(macName, "[0-9]+").Value.PadLeft(2, '0'));
                                    if (dbMac != null && dbMac.Id > 0)
                                        machineId = dbMac.Id;
                                    else
                                    {
                                        errorCount++;
                                        continue;
                                    }
                                }

                                using (MoldBO bObj = new MoldBO())
                                {
                                    int? nutQtyResult = null;
                                    int nutQty32 = 0;

                                    try
                                    {
                                        if (!Int32.TryParse(Regex.Match(nutQty, "[0-9]+").Value, out nutQty32))
                                            nutQtyResult = nutQty32;
                                    }
                                    catch (Exception)
                                    {

                                    }

                                    var nModel = new MoldTestModel
                                    {
                                        MachineId = machineId,
                                        ProductDescription = productDesc,
                                        HeadSize = headSize,
                                        RawMaterialName = rawMatName,
                                        TestDate = DateTime.Now,
                                        CreatedDate = DateTime.Now,
                                        
                                        DyeCode = dyeCode,
                                        RalCode = ralCode,
                                        PackageDimension = packSize,
                                        
                                        NutQuantity = nutQtyResult,
                                        NutCaliber = nutCaliber,
                                        
                                        ProductCode = productCode,
                                        ProductName = productName,
                                        MoldCode = moldCode,
                                        MoldName = moldName,
                                    };

                                    try
                                    {
                                        nModel.RawMaterialGr = Convert.ToInt32(rawInfo[0].Replace("(", ""));
                                        nModel.RawMaterialTolerationGr = Convert.ToInt32(rawInfo[1].Replace(")", ""));
                                    }
                                    catch (Exception)
                                    {

                                    }

                                    try
                                    {
                                        nModel.InflationTimeSeconds = Convert.ToInt32(Regex.Match(inflTime, "[0-9]+").Value);
                                        nModel.TotalTimeSeconds = Convert.ToInt32(Regex.Match(totalTime, "[0-9]+").Value);
                                    }
                                    catch (Exception)
                                    {

                                    }

                                    try
                                    {
                                        nModel.InPackageQuantity = Convert.ToInt32(Regex.Match(inPckQty, "[0-9]+").Value);
                                        nModel.InPalletPackageQuantity = Convert.ToInt32(Regex.Match(inPltQty, "[0-9]+").Value);
                                    }
                                    catch (Exception)
                                    {

                                    }

                                    var bResult = bObj.SaveOrUpdateMoldTest(nModel);

                                    if (bResult.Result)
                                        transferredCount++;
                                    else
                                        errorCount++;

                                    this.BeginInvoke((Action)delegate
                                    {
                                        lblSuccess.Text = "Başarılı: " + transferredCount.ToString();
                                        lblError.Text = "Başarısız: " + errorCount.ToString();
                                    });
                                }
                            }
                            catch (Exception ex)
                            {
                                lblErrMsg.Text = ex.Message;
                            }
                            
                        }
                    }

                    MessageBox.Show("Transfer tamamlandı.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}
