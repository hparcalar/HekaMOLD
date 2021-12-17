using HekaMOLD.Business.Models.DataTransfer.Core;
using HekaMOLD.Business.Models.DataTransfer.Production;
using HekaMOLD.Business.Models.DataTransfer.Receipt;
using HekaMOLD.Business.Models.Operational;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HekaMOLD.Business.IntSvc_WebTicari;
using HekaMOLD.Business.UseCases.Integrations.Params.WebTicari;
using System.Xml;
using Newtonsoft.Json;
using HekaMOLD.Business.Models.Constants;
using System.Text.RegularExpressions;

namespace HekaMOLD.Business.UseCases.Integrations
{
    public class WebTicariIntegrator : IntegratorBase
    {
        public event EventHandler OnTransferError;

        private WebTicariLoginConfiguration ResolveLoginData(string connectionData)
        {
            WebTicariLoginConfiguration data = new WebTicariLoginConfiguration();

            string[] dataParts = connectionData.Split(';');
            foreach (var item in dataParts)
            {
                var pairParts = item.Split('=');
                switch (pairParts[0])
                {
                    case "CustomerNo":
                        data.CustomerNo = pairParts[1];
                        break;
                    case "Login":
                        data.Login = pairParts[1];
                        break;
                    case "Password":
                        data.Password = pairParts[1];
                        break;
                    case "BranchOfficeNo":
                        data.BranchOfficeNo = pairParts[1];
                        break;
                    default:
                        break;
                }
            }

            return data;
        }

        public BusinessResult PullFirms(SyncPointModel syncPoint)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var loginConfig = ResolveLoginData(syncPoint.ConnectionString);

                using (WebticariService ws = new WebticariService())
                {
                    var wsToken = ws.login(loginConfig.CustomerNo, loginConfig.Login, loginConfig.Password);
                    string response = ws.exportCustomerXML(wsToken, "", "", "");

                    XmlDocument xmlResp = new XmlDocument();
                    xmlResp.LoadXml(response);
                    var jsonText = JsonConvert.SerializeXmlNode(xmlResp);
                    var jsonData = JsonConvert.DeserializeObject<dynamic>(jsonText);

                    foreach (var itemParent in jsonData.table.rows)
                    {
                        foreach (var item in itemParent.Value)
                        {
                            if (!string.IsNullOrEmpty(item.cr_no.Value) && item.cr_sube == loginConfig.BranchOfficeNo)
                            {
                                using (DefinitionsBO bObj = new DefinitionsBO())
                                {
                                    if (!bObj.HasAnyFirm(item.cr_kod.Value))
                                    {
                                        bObj.SaveOrUpdateFirm(new FirmModel
                                        {
                                            FirmCode = item.cr_kod.Value,
                                            FirmName = item.cr_adi.Value,
                                            FirmTitle = item.cr_unvan.Value,
                                            Address = !string.IsNullOrEmpty(item.cr_adres.Value) ?
                                                item.cr_adres.Value + ", İl:" + item.cr_il.Value : "",
                                            Phone = item.cr_tel.Value,
                                            Gsm = item.cr_cep.Value,
                                            PlantId = syncPoint.PlantId,
                                            TaxNo = !string.IsNullOrEmpty(item.cr_vergino.Value) ?
                                                item.cr_vergino.Value : item.cr_tckimlik.Value,
                                            TaxOffice = item.cr_vergidairesi.Value,
                                            FirmType = (int)FirmType.All,
                                        });
                                    }
                                }
                            }
                        }
                    }
                }

                result.Result = true;
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public BusinessResult PullItems(SyncPointModel syncPoint)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var loginConfig = ResolveLoginData(syncPoint.ConnectionString);

                using (WebticariService ws = new WebticariService())
                {
                    var wsToken = ws.login(loginConfig.CustomerNo, loginConfig.Login, loginConfig.Password);
                    string response = ws.exportStockXML(wsToken, "", "", "");

                    XmlDocument xmlResp = new XmlDocument();
                    xmlResp.LoadXml(response);
                    var jsonText = JsonConvert.SerializeXmlNode(xmlResp);
                    var jsonData = JsonConvert.DeserializeObject<dynamic>(jsonText);

                    foreach (var itemParent in jsonData.table.rows)
                    {
                        foreach (var item in itemParent.Value)
                        {
                            // SKIP PASSIVE ITEM RECORDS
                            if (item.ur_durum.Value != "A")
                                continue;

                            using (DefinitionsBO bObj = new DefinitionsBO())
                            {
                                if (item.ur_sube == loginConfig.BranchOfficeNo && !bObj.HasAnyItem(item.ur_kod.Value))
                                {
                                    int? unitTypeId = null;
                                    #region RESOLVE SYSTEM UNIT TYPE
                                    if (!string.IsNullOrEmpty(item.ur_kullbirim.Value))
                                    {
                                        using (DefinitionsBO subObj = new DefinitionsBO())
                                        {
                                            if (!subObj.HasAnyUnitType(item.ur_kullbirim.Value))
                                            {
                                                var crResult = subObj.SaveOrUpdateUnitType(new UnitTypeModel
                                                {
                                                    PlantId = syncPoint.PlantId,
                                                    UnitCode = item.ur_kullbirim.Value,
                                                    UnitName = item.ur_kullbirim.Value,
                                                });
                                                if (crResult.Result)
                                                    unitTypeId = crResult.RecordId;
                                            }
                                            else
                                            {
                                                var unitObj = subObj.GetUnitType(item.ur_kullbirim.Value);
                                                if (unitObj != null)
                                                    unitTypeId = unitObj.Id;
                                            }
                                        }
                                    }
                                    #endregion

                                    int? itemTypeNo = (int)ItemType.Commercial;
                                    #region RESOLVE ITEM TYPE
                                    if (Regex.IsMatch(item.ur_tipi.Value, "^HA$"))
                                        itemTypeNo = (int)ItemType.RawMaterials;
                                    else if (Regex.IsMatch(item.ur_tipi.Value, "^GS$|^GD$|^TM$|^MA$"))
                                        itemTypeNo = (int)ItemType.Product;
                                    else if (item.ur_tipi.Value == null
                                        || Regex.IsMatch(item.ur_tipi.Value, "^HZ$|^MH$|^YM$|^SM$|^MU$"))
                                        itemTypeNo = (int)ItemType.Commercial;
                                    #endregion

                                    int? itemGroupId = null;
                                    #region RESOLVE ITEM GROUP
                                    if (!string.IsNullOrEmpty(item.gr_adi.Value))
                                    {
                                        using (DefinitionsBO subObj = new DefinitionsBO())
                                        {
                                            if (!subObj.HasAnyItemGroup(item.gr_adi.Value))
                                            {
                                                var crResult = subObj.SaveOrUpdateItemGroup(new ItemGroupModel
                                                {
                                                    PlantId = syncPoint.PlantId,
                                                    ItemGroupCode = item.gr_adi.Value,
                                                    ItemGroupName = item.gr_adi.Value,
                                                });

                                                if (crResult.Result)
                                                    itemGroupId = crResult.RecordId;
                                            }
                                            else
                                            {
                                                var grObj = subObj.GetItemGroup(item.gr_adi.Value);
                                                if (grObj != null)
                                                    itemGroupId = grObj.Id;
                                            }
                                        }
                                    }
                                    #endregion

                                    bObj.SaveOrUpdateItem(new ItemModel
                                    {
                                        ItemNo = item.ur_kod.Value,
                                        ItemName = item.ur_adi.Value,
                                        PlantId = syncPoint.PlantId,
                                        ItemType = itemTypeNo,
                                        ItemGroupId = itemGroupId,
                                        // bi_alisfiyat,
                                        // bi_satisfiyat1, bi_satisfiyat5
                                        // kd_kdv,
                                        Units = unitTypeId > 0 ? new ItemUnitModel[]
                                        {
                                            new ItemUnitModel
                                            {
                                                IsMainUnit = true,
                                                DividerFactor=1,
                                                MultiplierFactor=1,
                                                UnitId = unitTypeId,
                                                NewDetail=true,
                                            }
                                        } : null,
                                    });
                                }
                            }
                        }
                    }
                }

                result.Result = true;
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public BusinessResult PullRecipes(SyncPointModel syncPoint)
        {
            BusinessResult result = new BusinessResult();

            try
            {

                result.Result = true;
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public BusinessResult PullSaleOrders(SyncPointModel syncPoint)
        {
            BusinessResult result = new BusinessResult();

            try
            {

                result.Result = true;
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public BusinessResult PullUnits(SyncPointModel syncPoint)
        {
            BusinessResult result = new BusinessResult();

            try
            {

                result.Result = true;
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public BusinessResult PushDeliveryReceipts(SyncPointModel syncPoint, ItemReceiptModel[] receipts)
        {
            BusinessResult result = new BusinessResult();

            try
            {

                result.Result = true;
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public BusinessResult PushFinishedProducts(SyncPointModel syncPoint, WorkOrderModel[] workOrders)
        {
            BusinessResult result = new BusinessResult();

            try
            {

                result.Result = true;
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public BusinessResult PushPurchasingWaybills(SyncPointModel syncPoint, ItemReceiptModel[] receipts)
        {
            BusinessResult result = new BusinessResult();

            try
            {

                result.Result = true;
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public BusinessResult PullEntryReceipts(SyncPointModel syncPoint)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var loginConfig = ResolveLoginData(syncPoint.ConnectionString);

                using (WebticariService ws = new WebticariService())
                {
                    var wsToken = ws.login(loginConfig.CustomerNo, loginConfig.Login, loginConfig.Password);
                    string response = ws.exportDataXML(wsToken, 
                        "SELECT * FROM tbalis{donem},tbasepet{donem}, tbcari, tbpersonel,tbdepo WHERE "
                        +" dp_no=sp_depo AND sp_satici=ps_no AND st_carino=cr_no AND st_id=sp_alisno AND cr_sube = " + loginConfig.BranchOfficeNo);

                    XmlDocument xmlResp = new XmlDocument();
                    xmlResp.LoadXml(response);
                    var jsonText = JsonConvert.SerializeXmlNode(xmlResp);
                    var jsonData = JsonConvert.DeserializeObject<dynamic>(jsonText);

                    foreach (var itemParent in jsonData.table.rows)
                    {
                        foreach (var item in itemParent.Value)
                        {
                            if (item.st_belgeturu == "ALIŞ FATURASI")
                            {
                                using (DefinitionsBO bObj = new DefinitionsBO())
                                {
                                    var dbFirm = bObj.GetFirm(item.cr_kod.ToString());
                                    var dbItem = bObj.GetItem(item.sp_urunkod.ToString());
                                    var dbUnit = bObj.GetUnitType(item.sp_birim.ToString());
                                    var dbWr = bObj.GetItemWarehouse();

                                    int? unitId = null;
                                    #region CHECK UNIT TYPE OR CREATE
                                    if (dbUnit == null)
                                    {
                                        using (DefinitionsBO bObjDec = new DefinitionsBO())
                                        {
                                            var decResult = bObjDec.SaveOrUpdateUnitType(new UnitTypeModel
                                            {
                                                PlantId = syncPoint.PlantId,
                                                UnitCode = item.sp_birim.ToString(),
                                                UnitName = item.sp_birim.ToString(),
                                            });

                                            if (decResult.Result)
                                                unitId = decResult.RecordId;
                                        }
                                    }
                                    else
                                        unitId = dbUnit.Id;
                                    #endregion

                                    if (dbFirm != null && dbItem != null && dbFirm.Id > 0 && dbItem.Id > 0)
                                    {
                                        var receiptNo = item.sp_alisno.ToString();
                                        var receiptDate = DateTime.ParseExact(item.st_tarih.ToString(), "yyyy-MM-dd HH:mm:ss",
                                            System.Globalization.CultureInfo.GetCultureInfo("tr"));
                                        var receiptQuantity = Convert.ToDecimal(Convert.ToSingle(item.sp_adet.ToString()));
                                        using (ReceiptBO bObjReceipt = new ReceiptBO())
                                        {
                                            ItemReceiptModel dbReceipt = bObjReceipt.FindItemEntryReceipt(receiptNo);
                                            if (dbReceipt == null)
                                            {
                                                dbReceipt = new ItemReceiptModel
                                                {
                                                    ReceiptDate = receiptDate,
                                                    ReceiptType = (int)ItemReceiptType.ItemBuying,
                                                    ReceiptNo = bObjReceipt.GetNextReceiptNo(syncPoint.PlantId.Value,
                                                        ItemReceiptType.ItemBuying),
                                                    PlantId = syncPoint.PlantId,
                                                    CreatedDate = DateTime.Now,
                                                    FirmId = dbFirm.Id,
                                                    ReceiptStatus = (int)ReceiptStatusType.Created,
                                                    SyncStatus = 1,
                                                    SyncDate = DateTime.Now,
                                                    InWarehouseId = dbWr.Id,
                                                };
                                            }

                                            dbReceipt.Details = new ItemReceiptDetailModel[]
                                            {
                                                new ItemReceiptDetailModel
                                                {
                                                    ItemId = dbItem.Id,
                                                    Quantity = receiptQuantity,
                                                    UnitId = unitId,
                                                    LineNumber = 1,
                                                    CreatedDate = DateTime.Now,
                                                    NewDetail = true,
                                                    ReceiptStatus = (int)ReceiptStatusType.Created,
                                                    SyncStatus = 1,
                                                    SyncDate = DateTime.Now,
                                                    TaxIncluded = false,
                                                    TaxRate = 0,
                                                    TaxAmount = 0,
                                                }
                                            };

                                            bObjReceipt.SaveOrUpdateItemReceipt(dbReceipt);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }


                result.Result = true;
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }
    }
}
