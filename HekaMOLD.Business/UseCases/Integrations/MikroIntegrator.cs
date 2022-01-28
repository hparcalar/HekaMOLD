using HekaMOLD.Business.Models.Constants;
using HekaMOLD.Business.Models.DataTransfer.Core;
using HekaMOLD.Business.Models.DataTransfer.Order;
using HekaMOLD.Business.Models.DataTransfer.Production;
using HekaMOLD.Business.Models.DataTransfer.Receipt;
using HekaMOLD.Business.Models.Operational;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HekaMOLD.Business.UseCases.Integrations
{
    public class MikroIntegrator : IntegratorBase
    {
        public event EventHandler OnTransferError;

        public BusinessResult PullFirms(SyncPointModel syncPoint)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                DataTable dTable = new DataTable();
                using (SqlConnection con = new SqlConnection(syncPoint.ConnectionString))
                {
                    con.Open();

                    SqlDataAdapter dAdapter = new SqlDataAdapter("SELECT * FROM CARI_HESAPLAR WHERE cari_iptal = 0", con);
                    dAdapter.Fill(dTable);
                    dAdapter.Dispose();
                    
                    foreach (DataRow row in dTable.Rows)
                    {
                        using (DefinitionsBO bObj = new DefinitionsBO())
                        {
                            if (!bObj.HasAnyFirm(row["cari_kod"].ToString()))
                            {
                                int firmType = (int)FirmType.All;
                                if (Convert.ToInt32(row["cari_hareket_tipi"]) == 1)
                                    firmType = (int)FirmType.Customer;
                                else if (Convert.ToInt32(row["cari_hareket_tipi"]) == 2)
                                    firmType = (int)FirmType.Supplier;

                                string addr1 = "", addr2 = "";

                                SqlCommand cmd = new SqlCommand("SELECT * FROM CARI_HESAP_ADRESLERI WHERE adr_cari_kod='"
                                    + row["cari_kod"] + "'", con);
                                SqlDataReader rdr = cmd.ExecuteReader();
                                try
                                {
                                    // FETCH ADDRESSES
                                    int subDataIndex = 0;
                                    while (rdr.Read())
                                    {
                                        string intAddr = rdr["adr_cadde"] + " "
                                            + rdr["adr_sokak"] + " " + rdr["adr_ilce"] + " / " + rdr["adr_il"];

                                        if (subDataIndex == 0)
                                            addr1 = intAddr;
                                        else if (subDataIndex == 1)
                                            addr2 = intAddr;
                                    }
                                }
                                catch (Exception)
                                {

                                }
                                finally
                                {
                                    if (rdr != null)
                                        rdr.Close();
                                    if (cmd != null)
                                        cmd.Dispose();
                                }

                                // FETCH AUTHORS
                                List<FirmAuthorModel> firmAuthors = new List<FirmAuthorModel>();
                                cmd = new SqlCommand("SELECT * FROM CARI_HESAP_YETKILILERI WHERE mye_cari_kod='" +
                                    row["cari_kod"] + "'", con);
                                rdr = cmd.ExecuteReader();

                                try
                                {
                                    while (rdr.Read())
                                    {
                                        firmAuthors.Add(new FirmAuthorModel
                                        {
                                            AuthorName = rdr["mye_isim"] + " " + rdr["mye_soyisim"],
                                            Email = rdr["mye_email_adres"].ToString(),
                                            Phone = rdr["mye_cep_telno"].ToString(),
                                            NewDetail = true,
                                            Title = "",
                                            SendMailForProduction = false,
                                            SendMailForPurchaseOrder = false,
                                        });
                                    }
                                }
                                catch (Exception)
                                {

                                }
                                finally
                                {
                                    rdr.Close();
                                    cmd.Dispose();
                                }

                                var firmModel = new FirmModel
                                {
                                    FirmCode = (string)row["cari_kod"],
                                    FirmName = (string)row["cari_unvan1"],
                                    PlantId = syncPoint.PlantId,
                                    CreatedDate = DateTime.Now,
                                    FirmType = firmType,
                                };

                                if (firmModel.FirmType == 0)
                                    firmModel.FirmType = 1;

                                try
                                {
                                    firmModel.FirmTitle = (string)row["cari_unvan1"];
                                    firmModel.TaxNo = (string)row["cari_vdaire_no"];
                                    firmModel.TaxOffice = (string)row["cari_vdaire_adi"];
                                    firmModel.Address = addr1;
                                    firmModel.Address2 = addr2;
                                    firmModel.Phone = (string)row["cari_CepTel"];
                                    firmModel.Email = (string)row["cari_EMail"];
                                    firmModel.Authors = firmAuthors.ToArray();
                                }
                                catch (Exception)
                                {

                                }
                                    
                                bObj.SaveOrUpdateFirm(firmModel);
                            }
                        }
                    }

                    con.Close();
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
                SystemParameterModel groupParams = null;
                Dictionary<string, int> groupKeys = null;
                
                using (DefinitionsBO bObj = new DefinitionsBO())
                {
                    var allParams = bObj.GetAllParameters(syncPoint.PlantId.Value);
                    if (allParams.Any(d => d.PrmCode == "IntegrationItemGroups"))
                        groupParams = allParams.First(d => d.PrmCode == "IntegrationItemGroups");
                }

                if (groupParams != null && !string.IsNullOrEmpty(groupParams.PrmValue))
                {
                    groupKeys = JsonConvert.DeserializeObject<Dictionary<string, int>>(groupParams.PrmValue);
                }

                DataTable dTable = new DataTable();
                using (SqlConnection con = new SqlConnection(syncPoint.ConnectionString))
                {
                    con.Open();

                    SqlDataAdapter dAdapter = new SqlDataAdapter("SELECT * FROM STOKLAR WHERE sto_iptal = 0 AND len(sto_isim) > 0", con);
                    dAdapter.Fill(dTable);
                    dAdapter.Dispose();

                    foreach (DataRow row in dTable.Rows)
                    {
                        using (DefinitionsBO bObj = new DefinitionsBO())
                        {
                            if (!bObj.HasAnyItem(row["sto_kod"].ToString()))
                            {
                                int itemType = (int)ItemType.Commercial;
                                if (Convert.ToInt32(row["sto_cins"]) == 1 || Convert.ToInt32(row["sto_cins"]) == 11)
                                    itemType = (int)ItemType.RawMaterials;
                                else if (Convert.ToInt32(row["sto_cins"]) == 2 || Convert.ToInt32(row["sto_cins"]) == 3)
                                    itemType = (int)ItemType.SemiProduct;
                                else if (Convert.ToInt32(row["sto_cins"]) == 4
                                    || Convert.ToInt32(row["sto_cins"]) == 5 || Convert.ToInt32(row["sto_cins"]) == 10)
                                    itemType = (int)ItemType.Product;

                                // SYSTEM PARAMETER GROUP ASSIGNMENT BY MIKRO ITEM CODE
                                int? properGroupId = null;
                                if (groupKeys.Any(d => row["sto_kod"].ToString().StartsWith(d.Key)))
                                {
                                    var properGroupKey = groupKeys.First(d => row["sto_kod"].ToString().StartsWith(d.Key));
                                    properGroupId = properGroupKey.Value;
                                }

                                int? categoryId = null;
                                // FETCH & UPDATE ITEM CATEGORY
                                try
                                {
                                    if (!string.IsNullOrEmpty(row["sto_kategori_kodu"].ToString()))
                                    {
                                        using (DefinitionsBO subObj = new DefinitionsBO())
                                        {
                                            if (!subObj.HasAnyItemCategory(row["sto_kategori_kodu"].ToString()))
                                            {
                                                SqlCommand cmd = new SqlCommand("SELECT * FROM STOK_KATEGORILERI WHERE ktg_kod='" +
                                                        row["sto_kategori_kodu"]
                                                    + "'", con);
                                                SqlDataReader rdr = cmd.ExecuteReader();
                                                if (rdr.Read())
                                                {
                                                    string catName = rdr["ktg_isim"].ToString();
                                                    var bResult = subObj.SaveOrUpdateItemCategory(new ItemCategoryModel
                                                    {
                                                        ItemCategoryCode = row["sto_kategori_kodu"].ToString(),
                                                        ItemCategoryName = catName,
                                                        PlantId = syncPoint.PlantId,
                                                        CreatedDate = DateTime.Now,
                                                    });
                                                    if (bResult.Result)
                                                        categoryId = bResult.RecordId;
                                                }
                                                rdr.Close();
                                                cmd.Dispose();
                                            }
                                            else
                                            {
                                                var dbItemCat = subObj.GetItemCategory(row["sto_kategori_kodu"].ToString());
                                                if (dbItemCat != null)
                                                    categoryId = dbItemCat.Id;
                                            }
                                        }
                                    }
                                }
                                catch (Exception)
                                {

                                }

                                int? groupId = null;
                                // FETCH & UPDATE ITEM GROUP
                                try
                                {
                                    if (!string.IsNullOrEmpty(row["sto_anagrup_kod"].ToString()))
                                    {
                                        using (DefinitionsBO subObj = new DefinitionsBO())
                                        {
                                            if (!subObj.HasAnyItemGroup(row["sto_anagrup_kod"].ToString()))
                                            {
                                                SqlCommand cmd = new SqlCommand("SELECT * FROM STOK_ANA_GRUPLARI WHERE san_kod='" +
                                                        row["sto_anagrup_kod"]
                                                    + "'", con);
                                                SqlDataReader rdr = cmd.ExecuteReader();
                                                if (rdr.Read())
                                                {
                                                    string groupName = rdr["san_isim"].ToString();
                                                    var bResult = subObj.SaveOrUpdateItemGroup(new ItemGroupModel
                                                    {
                                                        ItemGroupCode = row["sto_anagrup_kod"].ToString(),
                                                        ItemGroupName = groupName,
                                                        PlantId = syncPoint.PlantId,
                                                        CreatedDate = DateTime.Now,
                                                    });
                                                    if (bResult.Result)
                                                        groupId = bResult.RecordId;
                                                }
                                                rdr.Close();
                                                cmd.Dispose();
                                            }
                                            else
                                            {
                                                var dbItemGr = subObj.GetItemGroup(row["sto_anagrup_kod"].ToString());
                                                if (dbItemGr != null)
                                                    groupId = dbItemGr.Id;
                                            }
                                        }
                                    }
                                }
                                catch (Exception)
                                {

                                }

                                int? unitId = null;
                                List<ItemUnitModel> itemUnits = new List<ItemUnitModel>();
                                // FETCH & UPDATE UNITS
                                try
                                {
                                    if (!string.IsNullOrEmpty(row["sto_birim1_ad"].ToString()))
                                    {
                                        using (DefinitionsBO subObj = new DefinitionsBO())
                                        {
                                            if (!subObj.HasAnyUnitType(row["sto_birim1_ad"].ToString()))
                                            {
                                                var bResult = subObj.SaveOrUpdateUnitType(new UnitTypeModel
                                                {
                                                    PlantId = syncPoint.PlantId,
                                                    UnitCode = (string)row["sto_birim1_ad"],
                                                    UnitName = (string)row["sto_birim1_ad"],
                                                    CreatedDate = DateTime.Now,
                                                });

                                                if (bResult.Result)
                                                    unitId = bResult.RecordId;
                                            }
                                            else
                                            {
                                                var dbUnit = subObj.GetUnitType(row["sto_birim1_ad"].ToString());
                                                if (dbUnit != null)
                                                    unitId = dbUnit.Id;
                                            }
                                        }
                                    }
                                }
                                catch (Exception)
                                {

                                }


                                if (unitId != null)
                                {
                                    itemUnits.Add(new ItemUnitModel
                                    {
                                        UnitId = unitId,
                                        CreatedDate = DateTime.Now,
                                        IsMainUnit = true,
                                        DividerFactor = 1,
                                        MultiplierFactor = 1,
                                        NewDetail = true
                                    });
                                }

                                var itemResult = bObj.SaveOrUpdateItem(new ItemModel
                                {
                                    ItemNo = (string)row["sto_kod"],
                                    ItemName = (string)row["sto_isim"],
                                    ItemType = itemType,
                                    PlantId = syncPoint.PlantId,
                                    ItemCategoryId = categoryId,
                                    ItemGroupId = properGroupId != null ? properGroupId : groupId,
                                    Units = itemUnits.ToArray(),
                                });

                                if (!itemResult.Result && row["sto_kod"].ToString() == "152.02.9002.2009.1003")
                                    OnTransferError?.Invoke((string)row["sto_kod"] + ": " + itemResult.ErrorMessage, null);
                            }
                            else
                            {
                                using (DefinitionsBO bObjEx = new DefinitionsBO())
                                {
                                    var dbItem = bObjEx.GetItem(row["sto_kod"].ToString());
                                    if (dbItem != null)
                                    {
                                        // SYSTEM PARAMETER GROUP ASSIGNMENT BY MIKRO ITEM CODE
                                        int? properGroupId = null;
                                        if (groupKeys.Any(d => row["sto_kod"].ToString().StartsWith(d.Key)))
                                        {
                                            var properGroupKey = groupKeys.First(d => row["sto_kod"].ToString().StartsWith(d.Key));
                                            properGroupId = properGroupKey.Value;
                                        }

                                        bool needSave = false;
                                        if (dbItem.ItemGroupId != properGroupId && properGroupId != null)
                                        {
                                            dbItem.ItemGroupId = properGroupId;
                                            needSave = true;
                                        }

                                        string newItemName = (string)row["sto_isim"];
                                        if (newItemName != dbItem.ItemName)
                                        {
                                            dbItem.ItemName = newItemName;
                                            needSave = true;
                                        }

                                        if (needSave)
                                            bObjEx.SaveOrUpdateItem(dbItem);
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

        public BusinessResult PullRecipes(SyncPointModel syncPoint)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                DataTable dTable = new DataTable();
                using (SqlConnection con = new SqlConnection(syncPoint.ConnectionString))
                {
                    con.Open();

                    SqlDataAdapter dAdapter = new SqlDataAdapter("SELECT * FROM URUN_RECETELERI WHERE rec_iptal = 0 ORDER BY rec_anakod, rec_satirno", con);
                    dAdapter.Fill(dTable);
                    dAdapter.Dispose();

                    string lastRecipeNo = "";
                    int lastRecipeId = 0;
                    List<int> processedRecipes = new List<int>();

                    foreach (DataRow row in dTable.Rows)
                    {
                        // YENİ REÇETE BAŞLIĞI EKLENDİ VEYA GÜNCELLENDİ
                        if (lastRecipeNo != row["rec_anakod"].ToString())
                        {
                            lastRecipeId = 0;

                            int productId = 0;
                            using (DefinitionsBO defObj = new DefinitionsBO())
                            {
                                var dbProduct = defObj.GetItem(row["rec_anakod"].ToString());
                                if (dbProduct != null)
                                    productId = dbProduct.Id;
                            }

                            if (productId == 0)
                                continue;

                            using (RecipeBO subObj = new RecipeBO())
                            {
                                if (!subObj.HasAnyProductRecipe(row["rec_anakod"].ToString()))
                                {
                                    var recipeResult = subObj.SaveOrUpdateProductRecipe(new ProductRecipeModel
                                    {
                                        ProductRecipeCode = row["rec_anakod"].ToString(),
                                        CreatedDate = DateTime.Now,
                                        Description = row["rec_aciklama"].ToString(),
                                        IsActive = true,
                                        ProductId = productId,
                                        ProductRecipeType = 1,
                                        Details = new ProductRecipeDetailModel[0]
                                    });

                                    if (recipeResult.Result)
                                        lastRecipeId = recipeResult.RecordId;
                                    //else
                                    //    OnTransferError?.Invoke(recipeResult.ErrorMessage, null);
                                }
                                else
                                {
                                    var dbRecipe = subObj.GetProductRecipe(row["rec_anakod"].ToString());
                                    if (dbRecipe != null)
                                    {
                                        lastRecipeId = dbRecipe.Id;

                                        // MEVCUT REÇETE İSE GÜNCELLE VE TÜM DETAYLARINI SİL, AŞAĞIDA YENİDEN EKLENECEK
                                        dbRecipe.Description = row["rec_aciklama"].ToString();
                                        dbRecipe.Details = new ProductRecipeDetailModel[0];
                                        dbRecipe.ProductId = productId;
                                        subObj.SaveOrUpdateProductRecipe(dbRecipe);
                                    }
                                }
                            }

                            lastRecipeNo = row["rec_anakod"].ToString();
                        }

                        // AKTİF REÇETENİN DETAYLARINI GÜNCELLEMEYE DEVAM ET
                        if (lastRecipeId > 0)
                        {
                            using (RecipeBO recBO = new RecipeBO())
                            {
                                int? itemId = null, unitTypeId = null;
                                using (DefinitionsBO defObj = new DefinitionsBO())
                                {
                                    var dbItem = defObj.GetItem(row["rec_tuketim_kod"].ToString());
                                    if (dbItem != null)
                                    {
                                        itemId = dbItem.Id;
                                        unitTypeId = dbItem.Units
                                            .Where(d => d.IsMainUnit == true)
                                            .Select(d => d.UnitId).FirstOrDefault();
                                    }
                                }

                                if (itemId != null) {
                                    recBO.AddRecipeDetail(lastRecipeId, new ProductRecipeDetailModel
                                    {
                                        ItemId = itemId,
                                        ProcessType = Convert.ToInt32(row["rec_uretim_tuketim"]) == 0 ? 1 : 2, // 0: Tüketim, 1: Üretim
                                        UnitId = unitTypeId,
                                        Quantity = Decimal.Parse(row["rec_tuketim_miktar"].ToString(), System.Globalization.NumberStyles.Float),
                                        CreatedDate = DateTime.Now,
                                    });
                                }
                            }

                            // SİSTEMDEKİ REÇETE OLUP, MİKRODA OLMAYAN MALZEME VARSA; SİSTEMDEN SİL
                            if (!processedRecipes.Contains(lastRecipeId))
                            {
                                using (RecipeBO bObj = new RecipeBO())
                                {
                                    var dbRecipe = bObj.GetProductRecipe(lastRecipeId);
                                    if (dbRecipe != null)
                                    {
                                        var mikroItems = dTable.Select("[rec_anakod] = '" + row["rec_anakod"].ToString() + "'")
                                            .Select(d => d["rec_tuketim_kod"].ToString()).ToArray();

                                        var currentItems = dbRecipe.Details.ToList();
                                        var nonExistingItems = dbRecipe.Details.Where(d => !mikroItems.Contains(d.ItemNo))
                                            .ToArray();
                                        if (nonExistingItems.Length > 0)
                                        {
                                            foreach (var nonExsItem in nonExistingItems)
                                            {
                                                currentItems.Remove(nonExsItem);
                                            }
                                            dbRecipe.Details = currentItems.ToArray();

                                            bObj.SaveOrUpdateProductRecipe(dbRecipe);
                                        }
                                    }
                                }

                                processedRecipes.Add(lastRecipeId);
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

        public BusinessResult PullSaleOrders(SyncPointModel syncPoint)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                DateTime dtMinDelivery = DateTime.MinValue;
                using (ReceiptBO bObj = new ReceiptBO())
                {
                    var minDeliveryDate = bObj.GetParameter("MinimumSaleOrderDate", syncPoint.PlantId.Value);
                    if (minDeliveryDate != null && !string.IsNullOrEmpty(minDeliveryDate.PrmValue))
                    {
                        dtMinDelivery = DateTime.ParseExact(minDeliveryDate.PrmValue, "dd.MM.yyyy",
                            System.Globalization.CultureInfo.GetCultureInfo("tr"));
                    }
                }

                if (dtMinDelivery == DateTime.MinValue)
                    throw new Exception("Ürün çıkış irsaliyeleri için minimum başlangıç tarihi sistem parametreleri içerisinde belirtilmemiş.");

                DataTable dTable = new DataTable();
                using (SqlConnection con = new SqlConnection(syncPoint.ConnectionString))
                {
                    con.Open();

                    SqlDataAdapter dAdapter = new SqlDataAdapter("SELECT * FROM SIPARISLER WHERE sip_iptal=0 AND sip_miktar > sip_planlananmiktar AND " +
                        "sip_tip = 0 AND sip_kapat_fl = 0 AND sip_miktar > sip_teslim_miktar AND "
                        + "sip_tarih >= '" + string.Format("{0:yyyy-MM-dd} ", dtMinDelivery) + "' "
                        + " ORDER BY sip_evrakno_seri, sip_evrakno_sira, sip_satirno", con);
                    dAdapter.Fill(dTable);
                    dAdapter.Dispose();

                    string lastOrderNo = "";
                    int lastOrderId = 0;
                    int lineNumber = 1;

                    foreach (DataRow row in dTable.Rows)
                    {
                        bool isLockedOrder = false;
                        string intOrderNo = row["sip_evrakno_seri"].ToString() + row["sip_evrakno_sira"].ToString();

                        // YENİ SİPARİŞ BAŞLIĞI EKLENDİ VEYA GÜNCELLENDİ
                        if (lastOrderNo != intOrderNo)
                        {
                            lastOrderId = 0;
                            lineNumber = 1;

                            using (OrdersBO subObj = new OrdersBO())
                            {
                                if (!subObj.HasAnySaleOrder(intOrderNo))
                                {
                                    int? firmId = null;

                                    using (DefinitionsBO defObj = new DefinitionsBO())
                                    {
                                        var dbFirm = defObj.GetFirm(row["sip_musteri_kod"].ToString());
                                        if (dbFirm != null)
                                            firmId = dbFirm.Id;
                                    }

                                    if (firmId != null)
                                    {
                                        var orderResult = subObj.SaveOrUpdateItemOrder(new ItemOrderModel
                                        {
                                            OrderNo = subObj.GetNextOrderNo(syncPoint.PlantId.Value, ItemOrderType.Sale),
                                            DocumentNo = intOrderNo.ToString(),
                                            OrderType = (int)ItemOrderType.Sale,
                                            DateOfNeed = (DateTime)row["sip_teslim_tarih"],
                                            FirmId = firmId,
                                            PlantId = syncPoint.PlantId,
                                            OrderDate = (DateTime)row["sip_tarih"],
                                            OrderStatus = (int)OrderStatusType.Created,
                                            //SyncDate = DateTime.Now,
                                            //SyncStatus = 1,
                                            CreatedDate = DateTime.Now,
                                            Details = new ItemOrderDetailModel[0]
                                        }, detailCanBeNull: true);

                                        if (orderResult.Result)
                                            lastOrderId = orderResult.RecordId;
                                    }
                                }
                                else
                                {
                                    var dbItemOrder = subObj.GetItemOrder(intOrderNo, ItemOrderType.Sale);
                                    if (dbItemOrder != null)
                                    {
                                        if (dbItemOrder.SyncStatus == 2)
                                            continue;

                                        lastOrderId = dbItemOrder.Id;

                                        int? firmId = null;

                                        using (DefinitionsBO defObj = new DefinitionsBO())
                                        {
                                            var dbFirm = defObj.GetFirm(row["sip_musteri_kod"].ToString());
                                            if (dbFirm != null)
                                                firmId = dbFirm.Id;
                                        }

                                        dbItemOrder.FirmId = firmId;

                                        //if (dbItemOrder.OrderStatus != (int)OrderStatusType.Created)
                                        //    isLockedOrder = true;

                                        if (!isLockedOrder)
                                        {
                                            // MEVCUT SİPARİŞ İSE GÜNCELLE VE TÜM DETAYLARINI SİL, AŞAĞIDA YENİDEN EKLENECEK
                                            try
                                            {
                                                dbItemOrder.DateOfNeed = (DateTime)row["sip_teslim_tarih"];
                                            }
                                            catch (Exception)
                                            {

                                            }
                                            
                                            dbItemOrder.OrderDate = (DateTime)row["sip_tarih"];
                                            dbItemOrder.Details = new ItemOrderDetailModel[0];
                                            subObj.SaveOrUpdateItemOrder(dbItemOrder, detailCanBeNull: true);
                                        }
                                    }
                                }
                            }

                            lastOrderNo = intOrderNo;
                        }

                        // SİPARİŞ DETAYLARINI GÜNCELLE
                        if (!isLockedOrder)
                        {
                            using (OrdersBO orderBO = new OrdersBO())
                            {
                                int? itemId = null, unitTypeId = null, forexId=null;
                                using (DefinitionsBO defObj = new DefinitionsBO())
                                {
                                    var dbItem = defObj.GetItem(row["sip_stok_kod"].ToString());
                                    if (dbItem != null)
                                    {
                                        itemId = dbItem.Id;
                                        unitTypeId = dbItem.Units != null ? dbItem.Units
                                            .Where(d => d.IsMainUnit == true)
                                            .Select(d => d.UnitId).FirstOrDefault() : null;
                                    }

                                    if (Convert.ToInt32(row["sip_doviz_cinsi"]) > 0)
                                    {
                                        string forexTypeCode = "TL";
                                        switch (Convert.ToInt32(row["sip_doviz_cinsi"]))
                                        {
                                            case 0:
                                                forexTypeCode = "TL";
                                                break;
                                            case 1:
                                                forexTypeCode = "USD";
                                                break;
                                            case 2:
                                                forexTypeCode = "EUR";
                                                break;
                                            default:
                                                break;
                                        }

                                        var dbForex = defObj.GetForexType(forexTypeCode);
                                        if (dbForex != null)
                                            forexId = dbForex.Id;
                                    }
                                }

                                if (itemId != null)
                                {
                                    var dbNewOrder = orderBO.GetItemOrder(lastOrderId);
                                    if (dbNewOrder.Details.Any(m => m.ItemId == itemId))
                                    {
                                        var existingDetail = dbNewOrder.Details.FirstOrDefault(m => m.ItemId == itemId);
                                        if (existingDetail != null)
                                        {
                                            existingDetail.UnitPrice = Decimal.Parse(row["sip_b_fiyat"].ToString(), System.Globalization.NumberStyles.Float);
                                            existingDetail.Quantity = Decimal.Parse(row["sip_miktar"].ToString(), System.Globalization.NumberStyles.Float)
                                                - Decimal.Parse(row["sip_planlananmiktar"].ToString(), System.Globalization.NumberStyles.Float);
                                            existingDetail.SubTotal = Decimal.Parse(row["sip_tutar"].ToString(), System.Globalization.NumberStyles.Float);
                                            existingDetail.TaxAmount = Decimal.Parse(row["sip_vergi"].ToString(), System.Globalization.NumberStyles.Float);
                                            existingDetail.ForexRate = Decimal.Parse(row["sip_doviz_kuru"].ToString(), System.Globalization.NumberStyles.Float);
                                            existingDetail.ForexId = forexId;

                                            // IF THERE IS A FOREX TYPE, CONVERT UNIT PRICE BY FOREX
                                            if (existingDetail.ForexId > 0)
                                            {
                                                existingDetail.ForexUnitPrice = existingDetail.UnitPrice;
                                                existingDetail.UnitPrice = existingDetail.ForexUnitPrice * existingDetail.ForexRate;
                                            }

                                            orderBO.UpdateOrderDetail(existingDetail);
                                        }
                                    }
                                    else
                                    {
                                        var newOrderDetail = new ItemOrderDetailModel
                                        {
                                            ItemId = itemId,
                                            UnitId = unitTypeId,
                                            LineNumber = lineNumber,
                                            OrderStatus = (int)OrderStatusType.Created,
                                            UnitPrice = Decimal.Parse(row["sip_b_fiyat"].ToString(), System.Globalization.NumberStyles.Float),
                                            Quantity = Decimal.Parse(row["sip_miktar"].ToString(), System.Globalization.NumberStyles.Float)
                                            - Decimal.Parse(row["sip_planlananmiktar"].ToString(), System.Globalization.NumberStyles.Float),
                                            SubTotal = Decimal.Parse(row["sip_tutar"].ToString(), System.Globalization.NumberStyles.Float),
                                            TaxAmount = Decimal.Parse(row["sip_vergi"].ToString(), System.Globalization.NumberStyles.Float),
                                            TaxIncluded = false,
                                            NewDetail = true,
                                            SyncDate = DateTime.Now,
                                            SyncStatus = 1,
                                            ForexId = forexId,
                                            ForexRate = Decimal.Parse(row["sip_doviz_kuru"].ToString(), System.Globalization.NumberStyles.Float),
                                            CreatedDate = DateTime.Now,
                                        };

                                        // IF THERE IS A FOREX TYPE, CONVERT UNIT PRICE BY FOREX
                                        if (newOrderDetail.ForexId > 0)
                                        {
                                            newOrderDetail.ForexUnitPrice = newOrderDetail.UnitPrice;
                                            newOrderDetail.UnitPrice = newOrderDetail.ForexUnitPrice * newOrderDetail.ForexRate;
                                        }

                                        var dResult = orderBO.AddOrderDetail(lastOrderId, newOrderDetail);
                                    }

                                    lineNumber++;
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

        public BusinessResult PullUnits(SyncPointModel syncPoint)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                // GET MASTER SYNC POINT OF THIS WORK DATA, BECAUSE MICRO USES A MASTER DATABASE FOR UNIT DEFINITIONS
                using (DefinitionsBO bObj = new DefinitionsBO())
                {
                    var masterPoint = bObj.GetSyncPointList()
                        .FirstOrDefault(d => d.SyncPointCode == (syncPoint.SyncPointCode + "_MASTER")
                        && d.SyncPointType == (int)SyncPointType.MikroMaster && d.IsActive == true);
                    if (masterPoint != null)
                        syncPoint = masterPoint;
                }

                DataTable dTable = new DataTable();
                using (SqlConnection con = new SqlConnection(syncPoint.ConnectionString))
                {
                    con.Open();

                    SqlDataAdapter dAdapter = new SqlDataAdapter("SELECT DISTINCT unit_ismi FROM STOK_BIRIMLERI", con);
                    dAdapter.Fill(dTable);
                    dAdapter.Dispose();

                    foreach (DataRow row in dTable.Rows)
                    {
                        using (DefinitionsBO bObj = new DefinitionsBO())
                        {
                            if (!bObj.HasAnyUnitType(row["unit_ismi"].ToString()))
                            {
                                bObj.SaveOrUpdateUnitType(new UnitTypeModel
                                {
                                    PlantId = syncPoint.PlantId,
                                    CreatedDate = DateTime.Now,
                                    UnitCode = (string)row["unit_ismi"],
                                    UnitName = (string)row["unit_ismi"],
                                });
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

        public BusinessResult PushProductions(SyncPointModel syncPoint)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                DateTime dtMinProd = DateTime.MinValue;
                using (ReceiptBO bObj = new ReceiptBO())
                {
                    var minDeliveryDate = bObj.GetParameter("MinimumProductionDate", syncPoint.PlantId.Value);
                    if (minDeliveryDate != null && !string.IsNullOrEmpty(minDeliveryDate.PrmValue))
                    {
                        dtMinProd = DateTime.ParseExact(minDeliveryDate.PrmValue, "dd.MM.yyyy",
                            System.Globalization.CultureInfo.GetCultureInfo("tr"));
                    }
                }

                if (dtMinProd == DateTime.MinValue)
                    throw new Exception("Üretim fişlerinin MİKRO'ya aktarımı için minimum başlangıç tarihi sistem parametreleri içerisinde belirtilmemiş.");

                using (ReceiptBO bObj = new ReceiptBO())
                {
                    var receipts = bObj.GetNonSyncProductions().Where(d => d.ReceiptDate >= dtMinProd).ToArray();
                    if (receipts != null)
                    {
                        foreach (var rcp in receipts)
                        {
                            if (rcp.Details != null && rcp.Details.Length > 0)
                            {
                                DataTable dTable = new DataTable();
                                using (SqlConnection con = new SqlConnection(syncPoint.ConnectionString))
                                {
                                    con.Open();

                                    SqlDataAdapter dAdapter =
                                        new SqlDataAdapter("SELECT TOP 1 sth_evrakno_sira FROM STOK_HAREKETLERI WHERE sth_tip=0 AND sth_cins=7 AND sth_normal_iade=0 " +
                                            "AND sth_evraktip=7 ORDER BY sth_evrakno_sira DESC", con);
                                    dAdapter.Fill(dTable);
                                    dAdapter.Dispose();

                                    int newReceiptNo = 1;
                                    if (dTable.Rows.Count > 0 && dTable.Rows[0][0] != DBNull.Value)
                                    {
                                        var lastReceiptNo = Convert.ToInt32(dTable.Rows[0][0]);
                                        newReceiptNo = lastReceiptNo + 1;
                                    }

                                    foreach (var rdt in rcp.Details)
                                    {
                                        try
                                        {
                                            string sql = "INSERT INTO STOK_HAREKETLERI(sth_SpecRECno, sth_iptal, sth_fileid, sth_hidden, sth_kilitli, sth_degisti, sth_checksum, sth_create_user, sth_lastup_user, "
                                                + "sth_special1, sth_special2, sth_special3, sth_firmano, sth_subeno, sth_tarih, sth_tip, sth_cins, sth_normal_iade, sth_evraktip, "
                                                + "sth_evrakno_seri, sth_evrakno_sira, sth_satirno, sth_belge_no, sth_belge_tarih, sth_stok_kod, sth_isk_mas1, sth_isk_mas2, sth_isk_mas3, sth_isk_mas4, sth_isk_mas5, sth_isk_mas6, sth_isk_mas7, sth_isk_mas8, sth_isk_mas9, sth_isk_mas10, "
                                                + "sth_sat_iskmas1, sth_sat_iskmas2,sth_sat_iskmas3,sth_sat_iskmas4,sth_sat_iskmas5,sth_sat_iskmas6,sth_sat_iskmas7,sth_sat_iskmas8,sth_sat_iskmas9,sth_sat_iskmas10, "
                                                + "sth_pos_satis, sth_promosyon_fl, sth_cari_cinsi, sth_cari_kodu, sth_cari_grup_no, sth_plasiyer_kodu, sth_har_doviz_cinsi, sth_har_doviz_kuru, "
                                                + "sth_stok_doviz_cinsi, sth_stok_doviz_kuru, sth_miktar, sth_miktar2, sth_birim_pntr, sth_tutar, "
                                                + "sth_iskonto1,sth_iskonto2,sth_iskonto3,sth_iskonto4,sth_iskonto5,sth_iskonto6, sth_masraf1,sth_masraf2,sth_masraf3,sth_masraf4, "
                                                + "sth_vergi_pntr, sth_vergi, sth_masraf_vergi_pntr,sth_masraf_vergi,sth_netagirlik,sth_odeme_op, sth_aciklama, sth_sip_uid, sth_fat_uid, "
                                                + "sth_giris_depo_no, sth_cikis_depo_no, sth_malkbl_sevk_tarihi, sth_cari_srm_merkezi, sth_stok_srm_merkezi, sth_fis_tarihi, "
                                                + "sth_fis_sirano, sth_vergisiz_fl, sth_maliyet_ana, sth_maliyet_alternatif, sth_maliyet_orjinal, sth_adres_no, sth_parti_kodu, "
                                                + "sth_lot_no, sth_kons_uid, sth_proje_kodu, sth_exim_kodu, sth_otv_pntr, sth_otv_vergi, sth_brutagirlik, sth_disticaret_turu, sth_otvtutari, "
                                                + "sth_otvvergisiz_fl, sth_oiv_pntr, sth_oiv_vergi, sth_oivvergisiz_fl, sth_fiyat_liste_no, sth_oivtutari, sth_Tevkifat_turu, sth_nakliyedeposu, "
                                                + "sth_nakliyedurumu, sth_yetkili_uid, sth_taxfree_fl, sth_ilave_edilecek_kdv, sth_ismerkezi_kodu,sth_HareketGrupKodu1,sth_HareketGrupKodu2,sth_HareketGrupKodu3, "
                                                + "sth_Olcu1,sth_Olcu2,sth_Olcu3,sth_Olcu4,sth_Olcu5, sth_FormulMiktarNo,sth_FormulMiktar,sth_eirs_senaryo,sth_eirs_tipi, sth_teslim_tarihi, "
                                                + "sth_matbu_fl, sth_satis_fiyat_doviz_cinsi, sth_satis_fiyat_doviz_kuru, sth_alt_doviz_kuru) "
                                                + " VALUES('0', 0, 16, 0, 0, 0, 0, 3, 3, '','','', 0, 0, '"+ string.Format("{0:yyyy-MM-dd HH:mm}", rcp.ReceiptDate) +"', "
                                                +"'0', '7', '0', '7', 'QQ', '"+ newReceiptNo +"', "+ rdt.LineNumber +", '', '"+ string.Format("{0:yyyy-MM-dd HH:mm}", rcp.ReceiptDate) + "', "
                                                +"'"+ rdt.ItemNo +"', 0,0,0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,0, 0, 0, 0, '', 0, '', 0, 1, 0, 1, "+ 
                                                   string.Format("{0:0.00}", rdt.Quantity).Replace(",", ".") + " ,0, 1, 0, 0,0,0,0,0,0, 0,0,0,0, 0,0, 0,0,0,0, '', '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', "
                                                   +"1, 1, '"+ string.Format("{0:yyyy-MM-dd HH:mm}", rcp.ReceiptDate) + "', '','', '1899-12-30 00:00:00', 0, 0, 0,0,0, 0, '"+ rdt.ItemNo +"', "
                                                   + "'0', '00000000-0000-0000-0000-000000000000', '', '', 0,0,0,0,0,0,0,0,0,0,0,0,0,0, '00000000-0000-0000-0000-000000000000', 0,0, "
                                                   +"'','','','', 0,0,0,0,0, 0,0,0,0, '"+ string.Format("{0:yyyy-MM-dd HH:mm}", rcp.ReceiptDate) + "', 0, 0,0,0)";
                                            SqlCommand cmd = new SqlCommand(sql, con);
                                            int affectedRows = cmd.ExecuteNonQuery();
                                            if (affectedRows > 0)
                                            {
                                                bObj.SignDetailAsSynced(rdt.Id);
                                            }
                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                    }

                                    con.Close();
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

        public BusinessResult PushPurchasedItems(SyncPointModel syncPoint)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                DateTime dtMinProd = DateTime.MinValue;
                using (ReceiptBO bObj = new ReceiptBO())
                {
                    var minDeliveryDate = bObj.GetParameter("MinimumPurchaseDate", syncPoint.PlantId.Value);
                    if (minDeliveryDate != null && !string.IsNullOrEmpty(minDeliveryDate.PrmValue))
                    {
                        dtMinProd = DateTime.ParseExact(minDeliveryDate.PrmValue, "dd.MM.yyyy",
                            System.Globalization.CultureInfo.GetCultureInfo("tr"));
                    }
                }

                if (dtMinProd == DateTime.MinValue)
                    throw new Exception("Satınalma fişlerinin MİKRO'ya aktarımı için minimum başlangıç tarihi sistem parametreleri içerisinde belirtilmemiş.");

                using (ReceiptBO bObj = new ReceiptBO())
                {
                    var receipts = bObj.GetNonSyncPurchasedItems().Where(d => d.ReceiptDate >= dtMinProd).ToArray();
                    if (receipts != null)
                    {
                        foreach (var rcp in receipts)
                        {
                            if (rcp.Details != null && rcp.Details.Length > 0)
                            {
                                DataTable dTable = new DataTable();
                                using (SqlConnection con = new SqlConnection(syncPoint.ConnectionString))
                                {
                                    con.Open();

                                    SqlDataAdapter dAdapter =
                                        new SqlDataAdapter("SELECT TOP 1 sth_evrakno_sira FROM STOK_HAREKETLERI WHERE sth_tip=0 AND sth_cins=0 "
                                        +" AND sth_normal_iade=0 " +
                                            "AND sth_evraktip=13 AND sth_evrakno_seri='IRS' ORDER BY sth_evrakno_sira DESC", con);
                                    dAdapter.Fill(dTable);
                                    dAdapter.Dispose();

                                    int newReceiptNo = 1;
                                    if (dTable.Rows.Count > 0 && dTable.Rows[0][0] != DBNull.Value)
                                    {
                                        var lastReceiptNo = Convert.ToInt32(dTable.Rows[0][0]);
                                        newReceiptNo = lastReceiptNo + 1;
                                    }

                                    string textPartOfDocument = Regex.Match(rcp.DocumentNo, "[A-Z]+").Value;
                                    string numericPartOfDocument = Regex.Match(rcp.DocumentNo, "[0-9]+").Value;

                                    foreach (var rdt in rcp.Details)
                                    {
                                        try
                                        {
                                            string sql = "INSERT INTO STOK_HAREKETLERI(sth_SpecRECno, sth_iptal, sth_fileid, sth_hidden, sth_kilitli, sth_degisti, sth_checksum, sth_create_user, sth_lastup_user, "
                                                + "sth_special1, sth_special2, sth_special3, sth_firmano, sth_subeno, sth_tarih, sth_tip, sth_cins, sth_normal_iade, sth_evraktip, "
                                                + "sth_evrakno_seri, sth_evrakno_sira, sth_satirno, sth_belge_no, sth_belge_tarih, sth_stok_kod, sth_isk_mas1, sth_isk_mas2, sth_isk_mas3, sth_isk_mas4, sth_isk_mas5, sth_isk_mas6, sth_isk_mas7, sth_isk_mas8, sth_isk_mas9, sth_isk_mas10, "
                                                + "sth_sat_iskmas1, sth_sat_iskmas2,sth_sat_iskmas3,sth_sat_iskmas4,sth_sat_iskmas5,sth_sat_iskmas6,sth_sat_iskmas7,sth_sat_iskmas8,sth_sat_iskmas9,sth_sat_iskmas10, "
                                                + "sth_pos_satis, sth_promosyon_fl, sth_cari_cinsi, sth_cari_kodu, sth_cari_grup_no, sth_plasiyer_kodu, sth_har_doviz_cinsi, sth_har_doviz_kuru, "
                                                + "sth_stok_doviz_cinsi, sth_stok_doviz_kuru, sth_miktar, sth_miktar2, sth_birim_pntr, sth_tutar, "
                                                + "sth_iskonto1,sth_iskonto2,sth_iskonto3,sth_iskonto4,sth_iskonto5,sth_iskonto6, sth_masraf1,sth_masraf2,sth_masraf3,sth_masraf4, "
                                                + "sth_vergi_pntr, sth_vergi, sth_masraf_vergi_pntr,sth_masraf_vergi,sth_netagirlik,sth_odeme_op, sth_aciklama, sth_sip_uid, sth_fat_uid, "
                                                + "sth_giris_depo_no, sth_cikis_depo_no, sth_malkbl_sevk_tarihi, sth_cari_srm_merkezi, sth_stok_srm_merkezi, sth_fis_tarihi, "
                                                + "sth_fis_sirano, sth_vergisiz_fl, sth_maliyet_ana, sth_maliyet_alternatif, sth_maliyet_orjinal, sth_adres_no, sth_parti_kodu, "
                                                + "sth_lot_no, sth_kons_uid, sth_proje_kodu, sth_exim_kodu, sth_otv_pntr, sth_otv_vergi, sth_brutagirlik, sth_disticaret_turu, sth_otvtutari, "
                                                + "sth_otvvergisiz_fl, sth_oiv_pntr, sth_oiv_vergi, sth_oivvergisiz_fl, sth_fiyat_liste_no, sth_oivtutari, sth_Tevkifat_turu, sth_nakliyedeposu, "
                                                + "sth_nakliyedurumu, sth_yetkili_uid, sth_taxfree_fl, sth_ilave_edilecek_kdv, sth_ismerkezi_kodu,sth_HareketGrupKodu1,sth_HareketGrupKodu2,sth_HareketGrupKodu3, "
                                                + "sth_Olcu1,sth_Olcu2,sth_Olcu3,sth_Olcu4,sth_Olcu5, sth_FormulMiktarNo,sth_FormulMiktar,sth_eirs_senaryo,sth_eirs_tipi, sth_teslim_tarihi, "
                                                + "sth_matbu_fl, sth_satis_fiyat_doviz_cinsi, sth_satis_fiyat_doviz_kuru, sth_alt_doviz_kuru) "
                                                + " VALUES('0', 0, 16, 0, 0, 0, 0, 3, 3, '','','', 0, 0, '" + string.Format("{0:yyyy-MM-dd HH:mm}", rcp.ReceiptDate) + "', "
                                                + "'0', '0', '0', '13', '"+ textPartOfDocument +"', '" + numericPartOfDocument + "', " + rdt.LineNumber + ", '', '" + string.Format("{0:yyyy-MM-dd HH:mm}", rcp.ReceiptDate) + "', "
                                                + "'" + rdt.ItemNo + "', 0,0,0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,0, 0, 0, 0, '', 0, '', 0, 1, 0, 1, " +
                                                   string.Format("{0:0.00}", rdt.Quantity).Replace(",", ".") + " ,0, 1, 0, 0,0,0,0,0,0, 0,0,0,0, 0,0, 0,0,0,0, '', '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', "
                                                   + "1, 1, '" + string.Format("{0:yyyy-MM-dd HH:mm}", rcp.ReceiptDate) + "', '','', '1899-12-30 00:00:00', 0, 0, 0,0,0, 0, '" + rdt.ItemNo + "', "
                                                   + "'0', '00000000-0000-0000-0000-000000000000', '', '', 0,0,0,0,0,0,0,0,0,0,0,0,0,0, '00000000-0000-0000-0000-000000000000', 0,0, "
                                                   + "'','','','', 0,0,0,0,0, 0,0,0,0, '" + string.Format("{0:yyyy-MM-dd HH:mm}", rcp.ReceiptDate) + "', 0, 0,0,0)";
                                            SqlCommand cmd = new SqlCommand(sql, con);
                                            int affectedRows = cmd.ExecuteNonQuery();
                                            if (affectedRows > 0)
                                            {
                                                bObj.SignDetailAsSynced(rdt.Id);
                                            }
                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                    }

                                    con.Close();

                                    // UPDATE MIKRO DOCUMENT NO TO HEKA RECEIPT DOCUMENT NO
                                    //using (ReceiptBO rObj = new ReceiptBO())
                                    //{
                                    //    rObj.UpdateDocumentNo(rcp.Id, "IRS" + newReceiptNo.ToString());
                                    //}
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

        public BusinessResult PushDeliveryReceipts(SyncPointModel syncPoint, ItemReceiptModel[] receipts)
        {
            throw new NotImplementedException();
        }

        public BusinessResult PushFinishedProducts(SyncPointModel syncPoint, WorkOrderModel[] workOrders)
        {
            throw new NotImplementedException();
        }

        public BusinessResult PushPurchasingWaybills(SyncPointModel syncPoint, ItemReceiptModel[] receipts)
        {
            throw new NotImplementedException();
        }

        public BusinessResult PushSaleOrders(SyncPointModel syncPoint)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                using (OrdersBO bObj = new OrdersBO())
                {
                    var receipts = bObj.GetNonSyncOrders(ItemOrderType.Sale);
                    if (receipts != null)
                    {
                        foreach (var rcp in receipts)
                        {
                            if (rcp.Details != null && rcp.Details.Length > 0)
                            {
                                if (string.IsNullOrEmpty(rcp.DocumentNo))
                                    continue;

                                string docText = Regex.Match(rcp.DocumentNo, "[A-Z]+").Value;
                                string numericText = Regex.Match(rcp.DocumentNo, "[0-9]+").Value;
                                if (string.IsNullOrEmpty(docText))
                                    continue;
                                if (!string.IsNullOrEmpty(numericText))
                                    continue;

                                DataTable dTable = new DataTable();
                                using (SqlConnection con = new SqlConnection(syncPoint.ConnectionString))
                                {
                                    con.Open();
                                    
                                    SqlDataAdapter dAdapter =
                                        new SqlDataAdapter("SELECT TOP 1 sip_evrakno_sira FROM SIPARISLER WHERE sip_tip=0 AND sip_evrakno_seri='"+ docText +"'" +
                                            " ORDER BY sip_evrakno_sira DESC", con);
                                    dAdapter.Fill(dTable);
                                    dAdapter.Dispose();

                                    int newReceiptNo = 1;
                                    if (dTable.Rows.Count > 0 && dTable.Rows[0][0] != DBNull.Value)
                                    {
                                        var lastReceiptNo = Convert.ToInt32(dTable.Rows[0][0]);
                                        newReceiptNo = lastReceiptNo + 1;
                                    }

                                    int lineNumber = 0;
                                    foreach (var rdt in rcp.Details)
                                    {
                                        if (rdt.Quantity <= 0)
                                        {
                                            lineNumber++;
                                            continue;
                                        }

                                        try
                                        {
                                            int mikroForexId = 0;
                                            decimal forexRate = 1;

                                            #region RESOLVE MIKRO FOREX ID
                                            if ((rdt.ForexId ?? 0) > 0)
                                            {
                                                string forexTypeCode = "TL";

                                                using (DefinitionsBO bObjDef = new DefinitionsBO())
                                                {
                                                    var dbForexType = bObjDef.GetForexType(rdt.ForexId.Value);
                                                    if (dbForexType != null)
                                                    {
                                                        forexTypeCode = dbForexType.ForexTypeCode;
                                                        forexRate = rdt.ForexRate ?? 1;
                                                    }
                                                }

                                                switch (forexTypeCode)
                                                {
                                                    case "TL":
                                                        mikroForexId = 0;
                                                        break;
                                                    case "USD":
                                                        mikroForexId = 1;
                                                        break;
                                                    case "EUR":
                                                        mikroForexId = 2;
                                                        break;
                                                    default:
                                                        break;
                                                }
                                            }
                                            #endregion

                                            int taxRateId = 1;
                                            #region RESOLVE TAX RATE
                                            if (rdt.TaxRate == 1)
                                                taxRateId = 2;
                                            else if (rdt.TaxRate == 8)
                                                taxRateId = 3;
                                            else if (rdt.TaxRate == 18)
                                                taxRateId = 4;
                                            #endregion

                                            var cariGrupNo = mikroForexId > 0 ? 2 : 1;

                                            string mikroSipCins = docText == "IHR" ? "3" : "0";
                                            string paymentPlanCode = !string.IsNullOrEmpty(rcp.PaymentPlanCode) ?
                                                rcp.PaymentPlanCode : "0";

                                            string sql = "INSERT INTO SIPARISLER(sip_SpecRECno, sip_iptal, sip_fileid, sip_hidden, sip_kilitli, sip_degisti, sip_checksum, sip_create_user, sip_lastup_user, "
                                                + "sip_special1, sip_special2, sip_special3, sip_firmano, sip_subeno, sip_tarih, sip_tip, sip_cins, "
                                                + "sip_evrakno_seri, sip_evrakno_sira, sip_satirno, sip_belgeno, sip_belge_tarih, sip_stok_kod, sip_iskonto_1, sip_iskonto_2, sip_iskonto_3, sip_iskonto_4, sip_iskonto_5, sip_iskonto_6, sip_masraf_1, sip_masraf_2, sip_masraf_3, sip_masraf_4, "
                                                + "sip_isk1, sip_isk2,sip_isk3,sip_isk4,sip_isk5,sip_isk6,sip_mas1,sip_mas2,sip_mas3,sip_mas4, "
                                                + "sip_satici_kod, sip_musteri_kod, sip_teslim_miktar,"
                                                + "sip_doviz_cinsi, sip_doviz_kuru, sip_miktar, sip_birim_pntr, sip_tutar, "
                                                + "sip_iskonto1,sip_iskonto2,sip_iskonto3,sip_iskonto4,sip_iskonto5,sip_iskonto6, sip_masraf1,sip_masraf2,sip_masraf3,sip_masraf4, "
                                                + "sip_vergi_pntr, sip_vergi, sip_masvergi_pntr,sip_masvergi,sip_opno, sip_aciklama, sip_b_fiyat, sip_depono,"
                                                +" sip_stok_sormerk, sip_cari_sormerk,sip_harekettipi, sip_projekodu, sip_aciklama2,sip_vergisiz_fl,sip_kapat_fl,sip_promosyon_fl, "
                                                +" sip_teslimturu, sip_cagrilabilir_fl, sip_durumu, sip_planlananmiktar, sip_OnaylayanKulNo, sip_cari_grupno, sip_adresno, sip_alt_doviz_kuru, sip_prosip_uid, "
                                                +" sip_Exp_Imp_Kodu, sip_kar_orani, sip_stal_uid, sip_teklif_uid, sip_parti_kodu, sip_lot_no, sip_fiyat_liste_no, sip_Otv_Pntr, "
                                                +" sip_Otv_Vergi, sip_otvtutari, sip_OtvVergisiz_Fl, sip_paket_kod, sip_Rez_uid, sip_yetkili_uid, sip_kapatmanedenkod, "
                                                +" sip_gecerlilik_tarihi, sip_onodeme_evrak_tip, sip_onodeme_evrak_seri, sip_rezervasyon_miktari, sip_rezerveden_teslim_edilen, "
                                                + " sip_HareketGrupKodu1,sip_HareketGrupKodu2,sip_HareketGrupKodu3, sip_Olcu1,sip_Olcu2,sip_Olcu3,sip_Olcu4,sip_Olcu5, "
                                                + " sip_FormulMiktarNo, sip_FormulMiktar, sip_satis_fiyat_doviz_cinsi, sip_satis_fiyat_doviz_kuru, sip_eticaret_kanali, sip_onodeme_evrak_sira, sip_teslim_tarih) "
                                                + " VALUES('0', 0, 21, 0, 0, 0, 0, 3, 3, '','','', 0, 0, '" + string.Format("{0:yyyy-MM-dd} 00:00:00", rcp.OrderDate) + "', "
                                                + "'0', '"+ mikroSipCins +"', '"+ docText +"', '" + newReceiptNo + "', " + lineNumber + ", '', '" + string.Format("{0:yyyy-MM-dd} 00:00:00", rcp.OrderDate) + "', "
                                                + "'" + rdt.ItemNo + "', 0,0,0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,0, 'SATIŞ02', '" + rcp.FirmCode +"', 0, "+ mikroForexId +", "+ string.Format("{0:0.00}", 1).Replace(",", ".") + ", " +
                                                   string.Format("{0:0.00}", rdt.Quantity ?? 0).Replace(",", ".") + " , 1, '"+ string.Format("{0:0.00}", (rdt.UnitPrice) * rdt.Quantity).Replace(",",".") +"', 0,0,0,0,0,0, 0,0,0,0, "+ taxRateId.ToString() +",'"+ string.Format("{0:0.00}", rdt.TaxAmount).Replace(",", ".") + "', 0,0,'"+ paymentPlanCode +"', '', " +
                                                   string.Format("{0:0.00}", rdt.UnitPrice ?? 0).Replace(",", ".") +", 1, '','', 0, '','',0,0,0,'03',1,0,0,0, '"+ cariGrupNo + "', '1', '0', '00000000-0000-0000-0000-000000000000', "
                                                   + " '', '0', '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', '', '0', '0', '0', "
                                                   + " '0', '0', '0', '', '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', '', "
                                                   + " '1899-12-30 00:00:00', '0', '', '0', '0', '','','', 0,0,0,0,0, '0', '0', "+ mikroForexId +", "+ string.Format("{0:0.00}", 1).Replace(",", ".").Replace(",", ".") + ", "
                                                   +" '0', '0', '"+ string.Format("{0:yyyy-MM-dd} 00:00:00", rcp.OrderDate) + "')";
                                            SqlCommand cmd = new SqlCommand(sql, con);
                                            int affectedRows = cmd.ExecuteNonQuery();
                                            if (affectedRows > 0)
                                            {
                                                bObj.SignDetailAsSent(rdt.Id);
                                            }
                                        }
                                        catch (Exception ex)
                                        {

                                        }

                                        lineNumber++;
                                    }

                                    using (OrdersBO bObjHeader = new OrdersBO())
                                    {
                                        var orderHeader = bObjHeader.GetItemOrder(rcp.Id);
                                        orderHeader.DocumentNo = docText + newReceiptNo.ToString();
                                        bObjHeader.SaveOrUpdateItemOrder(orderHeader);
                                    }

                                    con.Close();
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

        public BusinessResult PullProductDeliveries(SyncPointModel syncPoint)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                DateTime dtMinDelivery = DateTime.MinValue;
                using (ReceiptBO bObj = new ReceiptBO())
                {
                    var minDeliveryDate = bObj.GetParameter("MinimumProductDeliveryDate", syncPoint.PlantId.Value);
                    if (minDeliveryDate != null && !string.IsNullOrEmpty(minDeliveryDate.PrmValue))
                    {
                        dtMinDelivery = DateTime.ParseExact(minDeliveryDate.PrmValue, "dd.MM.yyyy",
                            System.Globalization.CultureInfo.GetCultureInfo("tr"));
                    }
                }

                if (dtMinDelivery == DateTime.MinValue)
                    throw new Exception("Ürün çıkış irsaliyeleri için minimum başlangıç tarihi sistem parametreleri içerisinde belirtilmemiş.");

                using (SqlConnection con = new SqlConnection(syncPoint.ConnectionString))
                {
                    con.Open();

                    DataTable dTable = new DataTable();
                    string sql = "SELECT * FROM STOK_HAREKETLERI WHERE sth_tip=1 AND sth_cins IN(0,1,2,12) AND sth_normal_iade=0 " +
                                            "AND sth_evraktip=1 AND sth_tarih >= '" + string.Format("{0:yyyy-MM-dd} ", dtMinDelivery) + "' "
                                            + "ORDER BY sth_evrakno_seri, sth_evrakno_sira, sth_satirno";
                    SqlDataAdapter dAdapter =
                                        new SqlDataAdapter(sql, con);
                    dAdapter.Fill(dTable);
                    dAdapter.Dispose();

                    if (dTable.Rows.Count > 0)
                    {
                        string lastReceiptNo = "";
                        int lastReceiptId = 0;
                        int lineNumber = 1;

                        foreach (DataRow row in dTable.Rows)
                        {
                            string intReceiptNo = row["sth_evrakno_seri"].ToString() + row["sth_evrakno_sira"].ToString();

                            int productWarehouse = 0;
                            // ÜRÜN SATIRI DEĞİLSE ATLA VE DEVAM ET
                            bool isProductRow = true;
                            using (DefinitionsBO dfObj = new DefinitionsBO())
                            {
                                var productData = dfObj.GetItem(row["sth_stok_kod"].ToString());
                                if (productData == null)
                                    isProductRow = false;

                                var prodWarehouseData = dfObj.GetProductWarehouse();
                                if (prodWarehouseData != null)
                                    productWarehouse = prodWarehouseData.Id;
                            }

                            if (!isProductRow)
                                continue;

                            // YENİ SEVK İRSALİYESİ BAŞLIĞI EKLENDİ VEYA GÜNCELLENDİ
                            if (lastReceiptNo != intReceiptNo)
                            {
                                lastReceiptId = 0;
                                lineNumber = 1;

                                using (ReceiptBO subObj = new ReceiptBO())
                                {
                                    if (!subObj.HasAnySaleReceipt(intReceiptNo))
                                    {
                                        int? firmId = null;

                                        using (DefinitionsBO defObj = new DefinitionsBO())
                                        {
                                            var dbFirm = defObj.GetFirm(row["sth_cari_kodu"].ToString());
                                            if (dbFirm != null)
                                                firmId = dbFirm.Id;
                                        }

                                        if (firmId != null)
                                        {
                                            var receiptResult = subObj.SaveOrUpdateItemReceipt(new ItemReceiptModel
                                            {
                                                ReceiptNo = subObj.GetNextReceiptNo(syncPoint.PlantId.Value, ItemReceiptType.ItemSelling),
                                                DocumentNo = intReceiptNo.ToString(),
                                                ReceiptType = (int)ItemReceiptType.ItemSelling,
                                                FirmId = firmId,
                                                PlantId = syncPoint.PlantId,
                                                ReceiptDate = (DateTime)row["sth_tarih"],
                                                ReceiptStatus = (int)ReceiptStatusType.Created,
                                                InWarehouseId = productWarehouse,
                                                //SyncDate = DateTime.Now,
                                                //SyncStatus = 1,
                                                CreatedDate = DateTime.Now,
                                                Details = new ItemReceiptDetailModel[0]
                                            }, detailCanBeNull: true);

                                            if (receiptResult.Result)
                                                lastReceiptId = receiptResult.RecordId;
                                        }
                                    }
                                    else
                                    {
                                        var dbItemReceipt = subObj.GetItemReceipt(intReceiptNo, ItemReceiptType.ItemSelling);
                                        if (dbItemReceipt != null)
                                        {
                                            lastReceiptId = dbItemReceipt.Id;

                                            int? firmId = null;

                                            using (DefinitionsBO defObj = new DefinitionsBO())
                                            {
                                                var dbFirm = defObj.GetFirm(row["sth_cari_kodu"].ToString());
                                                if (dbFirm != null)
                                                    firmId = dbFirm.Id;
                                            }

                                            dbItemReceipt.FirmId = firmId;

                                            // MEVCUT SEVK İRSALİYESİ İSE BAŞLIK BİLGİLERİNİ GÜNCELLE VE DETAYLARA DOKUNMA
                                            dbItemReceipt.ReceiptDate = (DateTime)row["sth_tarih"];
                                            dbItemReceipt.Details = new ItemReceiptDetailModel[0];
                                            subObj.SaveOrUpdateItemReceipt(dbItemReceipt, detailCanBeNull: true, dontChangeDetails: true);
                                        }
                                    }
                                }

                                lastReceiptNo = intReceiptNo;
                            }

                            // İRSALİYE DETAYLARINI GÜNCELLE
                            using (ReceiptBO receiptBO = new ReceiptBO())
                            {
                                int? itemId = null, unitTypeId = null, forexId = null;
                                using (DefinitionsBO defObj = new DefinitionsBO())
                                {
                                    var dbItem = defObj.GetItem(row["sth_stok_kod"].ToString());
                                    if (dbItem != null)
                                    {
                                        itemId = dbItem.Id;
                                        unitTypeId = dbItem.Units != null ? dbItem.Units
                                            .Where(d => d.IsMainUnit == true)
                                            .Select(d => d.UnitId).FirstOrDefault() : null;
                                    }

                                    if (Convert.ToInt32(row["sth_har_doviz_cinsi"]) > 0)
                                    {
                                        string forexTypeCode = "TL";
                                        switch (Convert.ToInt32(row["sth_har_doviz_cinsi"]))
                                        {
                                            case 0:
                                                forexTypeCode = "TL";
                                                break;
                                            case 1:
                                                forexTypeCode = "USD";
                                                break;
                                            case 2:
                                                forexTypeCode = "EUR";
                                                break;
                                            default:
                                                break;
                                        }

                                        var dbForex = defObj.GetForexType(forexTypeCode);
                                        if (dbForex != null)
                                            forexId = dbForex.Id;
                                    }
                                }

                                if (itemId != null)
                                {
                                    var dbNewReceipt = receiptBO.GetItemReceipt(lastReceiptId);
                                    if (dbNewReceipt.Details.Any(m => m.ItemId == itemId))
                                    {
                                        var existingDetail = dbNewReceipt.Details.FirstOrDefault(m => m.ItemId == itemId && m.LineNumber == lineNumber);
                                        if (existingDetail != null)
                                        {
                                            existingDetail.Quantity = Decimal.Parse(row["sth_miktar"].ToString(), System.Globalization.NumberStyles.Float);
                                            existingDetail.SubTotal = Decimal.Parse(row["sth_tutar"].ToString(), System.Globalization.NumberStyles.Float);
                                            existingDetail.TaxAmount = Decimal.Parse(row["sth_vergi"].ToString(), System.Globalization.NumberStyles.Float);
                                            existingDetail.ForexRate = Decimal.Parse(row["sth_har_doviz_kuru"].ToString(), System.Globalization.NumberStyles.Float);
                                            existingDetail.ForexId = forexId;
                                            try
                                            {
                                                existingDetail.UnitPrice = existingDetail.SubTotal / existingDetail.Quantity;
                                            }
                                            catch (Exception)
                                            {

                                            }

                                            // IF THERE IS A FOREX TYPE, CONVERT UNIT PRICE BY FOREX
                                            if (existingDetail.ForexId > 0)
                                            {
                                                existingDetail.ForexUnitPrice = existingDetail.UnitPrice;
                                                existingDetail.UnitPrice = existingDetail.ForexUnitPrice * existingDetail.ForexRate;
                                            }

                                            receiptBO.UpdateReceiptDetail(existingDetail);
                                        }
                                    }
                                    else
                                    {
                                        var newReceiptDetail = new ItemReceiptDetailModel
                                        {
                                            ItemId = itemId,
                                            UnitId = unitTypeId,
                                            LineNumber = lineNumber,
                                            ReceiptStatus = (int)ReceiptStatusType.Created,
                                            //UnitPrice = Decimal.Parse(row["sip_b_fiyat"].ToString(), System.Globalization.NumberStyles.Float),
                                            Quantity = Decimal.Parse(row["sth_miktar"].ToString(), System.Globalization.NumberStyles.Float),
                                            SubTotal = Decimal.Parse(row["sth_tutar"].ToString(), System.Globalization.NumberStyles.Float),
                                            TaxAmount = Decimal.Parse(row["sth_vergi"].ToString(), System.Globalization.NumberStyles.Float),
                                            TaxIncluded = false,
                                            NewDetail = true,
                                            SyncDate = DateTime.Now,
                                            SyncStatus = 1,
                                            ForexId = forexId,
                                            ForexRate = Decimal.Parse(row["sth_har_doviz_kuru"].ToString(), System.Globalization.NumberStyles.Float),
                                            CreatedDate = DateTime.Now,
                                        };

                                        try
                                        {
                                            newReceiptDetail.UnitPrice = newReceiptDetail.SubTotal / newReceiptDetail.Quantity;
                                        }
                                        catch (Exception)
                                        {

                                        }

                                        // IF THERE IS A FOREX TYPE, CONVERT UNIT PRICE BY FOREX
                                        if (newReceiptDetail.ForexId > 0)
                                        {
                                            newReceiptDetail.ForexUnitPrice = newReceiptDetail.UnitPrice;
                                            newReceiptDetail.UnitPrice = newReceiptDetail.ForexUnitPrice * newReceiptDetail.ForexRate;
                                        }

                                        var dResult = receiptBO.AddReceiptDetail(lastReceiptId, newReceiptDetail);
                                    }

                                    lineNumber++;
                                }
                            }
                        }
                    }

                    con.Close();
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

        public BusinessResult PullEntryReceipts(SyncPointModel syncPoint)
        {
            throw new NotImplementedException();
        }
    }
}
