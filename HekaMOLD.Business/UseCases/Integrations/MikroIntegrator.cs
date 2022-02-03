using HekaMOLD.Business.Models.Constants;
using HekaMOLD.Business.Models.DataTransfer.Core;
using HekaMOLD.Business.Models.DataTransfer.Order;
using HekaMOLD.Business.Models.DataTransfer.Production;
using HekaMOLD.Business.Models.DataTransfer.Receipt;
using HekaMOLD.Business.Models.Operational;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
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
                                    ItemGroupId = groupId,
                                    Units = itemUnits.ToArray(),
                                });

                                if (!itemResult.Result && row["sto_kod"].ToString() == "152.02.9002.2009.1003")
                                    OnTransferError?.Invoke((string)row["sto_kod"] + ": " + itemResult.ErrorMessage, null);
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
                DataTable dTable = new DataTable();
                using (SqlConnection con = new SqlConnection(syncPoint.ConnectionString))
                {
                    con.Open();

                    SqlDataAdapter dAdapter = new SqlDataAdapter("SELECT * FROM SIPARISLER WHERE sip_iptal=0 AND sip_miktar > sip_planlananmiktar AND " +
                        "sip_tip = 0 AND sip_kapat_fl = 0 AND sip_miktar > sip_teslim_miktar ORDER BY sip_evrakno_seri, sip_evrakno_sira, sip_satirno", con);
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

                                    //if (firmId != null)
                                    //{
                                    //    var orderResult = subObj.SaveOrUpdateItemOrder(new ItemOrderModel
                                    //    {
                                    //        OrderNo = subObj.GetNextOrderNo(syncPoint.PlantId.Value, ItemOrderType.Sale),
                                    //        DocumentNo = intOrderNo.ToString(),
                                    //        OrderType = (int)ItemOrderType.Sale,
                                    //        DateOfNeed = (DateTime)row["sip_teslim_tarih"],
                                    //        CustomerFirmId = firmId,
                                    //        PlantId = syncPoint.PlantId,
                                    //        OrderDate = (DateTime)row["sip_tarih"],
                                    //        OrderStatus = (int)OrderStatusType.Created,
                                    //        CreatedDate = DateTime.Now,
                                    //        Details = new ItemOrderDetailModel[0]
                                    //    }, detailCanBeNull: true);

                                    //    if (orderResult.Result)
                                    //        lastOrderId = orderResult.RecordId;
                                    //}
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
                using (ReceiptBO bObj = new ReceiptBO())
                {
                    var receipts = bObj.GetNonSyncProductions();
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
                                                + "sth_matbu_fl, sth_satis_fiyat_doviz_cinsi, sth_satis_fiyat_doviz_kuru) "
                                                + " VALUES('0', 0, 16, 0, 0, 0, 0, 3, 3, '','','', 0, 0, '"+ string.Format("{0:yyyy-MM-dd HH:mm}", rcp.ReceiptDate) +"', "
                                                +"'0', '7', '0', '7', 'QQ', '"+ newReceiptNo +"', "+ rdt.LineNumber +", '', '"+ string.Format("{0:yyyy-MM-dd HH:mm}", rcp.ReceiptDate) + "', "
                                                +"'"+ rdt.ItemNo +"', 0,0,0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,0, 0, 0, 0, '', 0, '', 0, 1, 0, 1, "+ 
                                                   string.Format("{0:0.00}", rdt.Quantity).Replace(",", ".") + " ,0, 1, 0, 0,0,0,0,0,0, 0,0,0,0, 0,0, 0,0,0,0, '', '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', "
                                                   +"1, 1, '"+ string.Format("{0:yyyy-MM-dd HH:mm}", rcp.ReceiptDate) + "', '','', '1899-12-30 00:00:00', 0, 0, 0,0,0, 0, '"+ rdt.ItemNo +"', "
                                                   + "'0', '00000000-0000-0000-0000-000000000000', '', '', 0,0,0,0,0,0,0,0,0,0,0,0,0,0, '00000000-0000-0000-0000-000000000000', 0,0, "
                                                   +"'','','','', 0,0,0,0,0, 0,0,0,0, '"+ string.Format("{0:yyyy-MM-dd HH:mm}", rcp.ReceiptDate) + "', 0, 0,0)";
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
    }
}
