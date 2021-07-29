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
                                rdr.Close();
                                cmd.Dispose();

                                // FETCH AUTHORS
                                List<FirmAuthorModel> firmAuthors = new List<FirmAuthorModel>();
                                cmd = new SqlCommand("SELECT * FROM CARI_HESAP_YETKILILERI WHERE mye_cari_kod='" +
                                    row["cari_kod"] + "'", con);
                                rdr = cmd.ExecuteReader();

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
                                rdr.Close();
                                cmd.Dispose();

                                bObj.SaveOrUpdateFirm(new FirmModel
                                {
                                    FirmCode = (string)row["cari_kod"],
                                    FirmName = (string)row["cari_unvan1"],
                                    FirmTitle = (string)row["cari_unvan1"],
                                    PlantId = syncPoint.PlantId,
                                    CreatedDate = DateTime.Now,
                                    FirmType = firmType,
                                    TaxNo = (string)row["cari_vdaire_no"],
                                    TaxOffice = (string)row["cari_vdaire_adi"],
                                    Address = addr1,
                                    Address2 = addr2,
                                    Phone = (string)row["cari_CepTel"],
                                    Email = (string)row["cari_EMail"],
                                    Authors = firmAuthors.ToArray()
                                });
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

                                int? groupId = null;
                                // FETCH & UPDATE ITEM GROUP
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

                                int? unitId = null;
                                List<ItemUnitModel> itemUnits = new List<ItemUnitModel>();
                                // FETCH & UPDATE UNITS
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

                                if (unitId != null)
                                {
                                    itemUnits.Add(new ItemUnitModel
                                    {
                                        UnitId = unitId,
                                        CreatedDate =DateTime.Now,
                                        IsMainUnit=true,
                                        DividerFactor=1,
                                        MultiplierFactor=1,
                                        NewDetail=true
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

                                //if (!itemResult.Result)
                                //    OnTransferError?.Invoke((string)row["sto_kod"] + ": " +itemResult.ErrorMessage, null);
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
                                        ProcessType = Convert.ToInt32(row["rec_uretim_tuketim"]), // 0: Tüketim, 1: Üretim
                                        UnitId = unitTypeId,
                                        Quantity = Convert.ToDecimal(row["rec_tuketim_miktar"].ToString().Replace(",",".")),
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

                    SqlDataAdapter dAdapter = new SqlDataAdapter("SELECT * FROM SIPARISLER WHERE sip_iptal=0 AND " +
                        "sip_cins = 0 ORDER BY sip_evrakno_seri, sip_evrakno_sira, sip_satirno", con);
                    dAdapter.Fill(dTable);
                    dAdapter.Dispose();

                    string lastOrderNo = "";
                    int lastOrderId = 0;

                    foreach (DataRow row in dTable.Rows)
                    {
                        string intOrderNo = row["sip_evrakno_seri"].ToString() + row["sip_evrakno_sira"].ToString();

                        // YENİ SİPARİŞ BAŞLIĞI EKLENDİ VEYA GÜNCELLENDİ
                        if (lastOrderNo != intOrderNo)
                        {
                            lastOrderId = 0;

                            using (OrdersBO subObj = new OrdersBO())
                            {
                                if (!subObj.HasAnySaleOrder(intOrderNo))
                                {
                                    var orderResult = subObj.SaveOrUpdateItemOrder(new ItemOrderModel
                                    {
                                        OrderNo = subObj.GetNextOrderNo(syncPoint.PlantId.Value, ItemOrderType.Sale),
                                        DocumentNo = intOrderNo.ToString(),
                                        OrderType = (int)ItemOrderType.Sale,
                                        DateOfNeed = (DateTime)row["sip_teslim_tarih"],
                                        OrderDate = (DateTime)row["sip_tarih"],
                                        CreatedDate = DateTime.Now,
                                        Details = new ItemOrderDetailModel[0]
                                    });

                                    if (orderResult.Result)
                                        lastOrderId = orderResult.RecordId;
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
                                        subObj.SaveOrUpdateProductRecipe(dbRecipe);
                                    }
                                }
                            }

                            lastRecipeNo = row["rec_anakod"].ToString();
                        }
                    }
                }

                result.Result = true;
            } catch (Exception ex)
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
