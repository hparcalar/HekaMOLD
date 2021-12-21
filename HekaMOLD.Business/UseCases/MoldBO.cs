using Heka.DataAccess.Context;
using HekaMOLD.Business.Helpers;
using HekaMOLD.Business.Models.Constants;
using HekaMOLD.Business.Models.DataTransfer.Core;
using HekaMOLD.Business.Models.DataTransfer.MoldTrace;
using HekaMOLD.Business.Models.DataTransfer.Production;
using HekaMOLD.Business.Models.Operational;
using HekaMOLD.Business.UseCases.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.UseCases
{
    public class MoldBO : CoreProductionBO
    {
        #region MOLD BUSINESS
        public ItemModel[] GetItemsOfMold(int moldId)
        {
            ItemModel[] data = new ItemModel[0];

            try
            {
                var repo = _unitOfWork.GetRepository<Item>();
                data = repo.Filter(d => d.MoldId == moldId)
                    .Select(d => new ItemModel
                    {
                        Id = d.Id,
                        ItemNo = d.ItemNo,
                        ItemName = d.ItemName,
                        GroupName = d.ItemGroup != null ? d.ItemGroup.ItemGroupName : "",
                        CategoryName = d.ItemCategory != null ? d.ItemCategory.ItemCategoryName : "",
                    }).ToArray();
            }
            catch (Exception)
            {

            }

            return data;
        }
        
        public MoldMoveHistory[] GetMoldMovementHistory(int moldId)
        {
            MoldMoveHistory[] data = new MoldMoveHistory[0];

            try
            {
                var repoMold = _unitOfWork.GetRepository<Mold>();
                var repoItem = _unitOfWork.GetRepository<Item>();
                var repoReceipt = _unitOfWork.GetRepository<ItemReceiptDetail>();

                var dbMold = repoMold.Get(d => d.Id == moldId);
                var dbItem = repoItem.Get(d => d.ItemNo == dbMold.MoldCode);
                data = repoReceipt.Filter(d => d.ItemId == dbItem.Id).ToList()
                    .Select(d => new MoldMoveHistory
                    {
                        ItemReceiptDetailId = d.Id,
                        ItemReceiptId = d.ItemReceiptId,
                        ReceiptCategory = (int)ReceiptCategoryType.ItemManagement,
                        WarehouseCode = d.ItemReceipt.Warehouse.WarehouseCode,
                        WarehouseName = d.ItemReceipt.Warehouse.WarehouseName,
                        InvoiceId = d.ItemReceipt != null ? d.ItemReceipt.InvoiceId : (int?)null,
                        FirmCode = d.ItemReceipt != null && d.ItemReceipt.Firm != null ? d.ItemReceipt.Firm.FirmCode : "",
                        FirmName = d.ItemReceipt != null && d.ItemReceipt.Firm != null ? d.ItemReceipt.Firm.FirmName : "",
                        InvoiceNo = d.ItemReceipt != null && d.ItemReceipt.Invoice != null ? d.ItemReceipt.Invoice.InvoiceNo : "",
                        InvoiceDocumentNo = d.ItemReceipt != null && d.ItemReceipt.Invoice != null ? d.ItemReceipt.Invoice.DocumentNo : "",
                        InvoiceDate = d.ItemReceipt != null && d.ItemReceipt.Invoice != null ? d.ItemReceipt.Invoice.InvoiceDate : null,
                        InvoiceDateStr = d.ItemReceipt != null && d.ItemReceipt.Invoice != null ? 
                            string.Format("{0:dd.MM.yyyy}", d.ItemReceipt.Invoice.InvoiceDate) : "",
                        InvoiceTypeStr = d.ItemReceipt != null && d.ItemReceipt.Invoice != null ? 
                            ((ItemReceiptType)d.ItemReceipt.Invoice.InvoiceType).ToCaption() : "",
                        ReceiptTypeStr = d.ItemReceipt != null ?
                            ((ItemReceiptType)d.ItemReceipt.ReceiptType).ToCaption() : "",
                        ReceiptDateStr = d.ItemReceipt != null ?
                            string.Format("{0:dd.MM.yyyy}", d.ItemReceipt.ReceiptDate) : "",
                        ReceiptNo = d.ItemReceipt != null ? d.ItemReceipt.ReceiptNo : "",
                        ReceiptDate = d.ItemReceipt != null ? d.ItemReceipt.ReceiptDate : null,
                        ReceiptDocumentNo = d.ItemReceipt != null ? d.ItemReceipt.DocumentNo : "",
                    })
                    .OrderByDescending(d => d.ReceiptDate)
                    .ToArray();
            }
            catch (Exception)
            {

            }

            return data;
        }

        public BusinessResult SetMoldStatus(int moldId, MoldStatus status, int? userId = null)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<Mold>();
                var dbMold = repo.Get(d => d.Id == moldId);
                if (dbMold == null)
                    throw new Exception("Kalıp tanımı bulunamadı.");

                dbMold.MoldStatus = (int)status;
                dbMold.UpdatedDate = DateTime.Now;
                dbMold.UpdatedUserId = userId;

                _unitOfWork.SaveChanges();

                result.Result = true;
                result.RecordId = dbMold.Id;
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public BusinessResult SendToRevision(int moldId, int firmId, int? userId = null, DateTime? moveDate = null)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<Mold>();
                var repoItem = _unitOfWork.GetRepository<Item>();

                var dbMold = repo.Get(d => d.Id == moldId);
                if (dbMold == null)
                    throw new Exception("Kalıp tanımı bulunamadı.");

                bool moldItemSet = false;
                if (dbMold.ItemMold == null)
                {
                    var dbItem = repoItem.Get(d => d.ItemNo == dbMold.MoldCode);
                    if (dbItem != null)
                    {
                        dbMold.MoldItemId = dbItem.Id;
                        moldItemSet = true;
                    }
                }

                using (ReceiptBO bObj = new ReceiptBO())
                {
                    result = bObj.SaveOrUpdateItemReceipt(new Models.DataTransfer.Receipt.ItemReceiptModel
                    {
                        ReceiptDate = moveDate ?? DateTime.Now,
                        ReceiptType = (int)ItemReceiptType.WarehouseOutput,
                        ReceiptNo = bObj.GetNextReceiptNo(dbMold.PlantId.Value, ItemReceiptType.WarehouseOutput),
                        PlantId = dbMold.PlantId,
                        CreatedUserId = userId,
                        CreatedDate = DateTime.Now,
                        InWarehouseId = dbMold.InWarehouseId,
                        FirmId = firmId,
                        ReceiptStatus = (int)ReceiptStatusType.Created,
                        Details = new Models.DataTransfer.Receipt.ItemReceiptDetailModel[] { 
                            new Models.DataTransfer.Receipt.ItemReceiptDetailModel
                            {
                                NewDetail = true,
                                Quantity = 1,
                                UnitId = null,
                                LineNumber = 1,
                                CreatedDate = DateTime.Now,
                                CreatedUserId = userId,
                                ItemId = dbMold.MoldItemId,
                                ReceiptStatus = (int)ReceiptStatusType.Created,
                                UnitPrice = 0,
                            }
                        },
                    });
                }

                // SAVE CHANGES FOR UPDATING MOLD ITEM RELATION
                if (moldItemSet)
                    _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public BusinessResult BackFromRevision(int moldId, int? userId = null, DateTime? moveDate = null)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<Mold>();
                var repoItem = _unitOfWork.GetRepository<Item>();
                var repoReceiptDetail = _unitOfWork.GetRepository<ItemReceiptDetail>();

                var dbMold = repo.Get(d => d.Id == moldId);
                if (dbMold == null)
                    throw new Exception("Kalıp tanımı bulunamadı.");

                bool moldItemSet = false;
                if (dbMold.ItemMold == null)
                {
                    var dbItem = repoItem.Get(d => d.ItemNo == dbMold.MoldCode);
                    if (dbItem != null)
                    {
                        dbMold.MoldItemId = dbItem.Id;
                        moldItemSet = true;
                    }
                }

                var dbSentReceipt = repoReceiptDetail.Filter(d => d.ItemId == dbMold.MoldItemId &&
                    d.ItemReceipt.ReceiptType > 100).OrderByDescending(d => d.ItemReceipt.ReceiptDate)
                    .FirstOrDefault();

                if (dbSentReceipt == null)
                    throw new Exception("Revizyona gönderilen kayıt bilgisi bulunamadı.");

                using (ReceiptBO bObj = new ReceiptBO())
                {
                    result = bObj.SaveOrUpdateItemReceipt(new Models.DataTransfer.Receipt.ItemReceiptModel
                    {
                        ReceiptDate = moveDate ?? DateTime.Now,
                        ReceiptType = (int)ItemReceiptType.WarehouseInput,
                        ReceiptNo = bObj.GetNextReceiptNo(dbMold.PlantId.Value, ItemReceiptType.WarehouseInput),
                        PlantId = dbMold.PlantId,
                        CreatedUserId = userId,
                        CreatedDate = DateTime.Now,
                        InWarehouseId = dbSentReceipt.ItemReceipt.InWarehouseId,
                        FirmId = dbSentReceipt.ItemReceipt.FirmId,
                        ReceiptStatus = (int)ReceiptStatusType.Created,
                        Details = new Models.DataTransfer.Receipt.ItemReceiptDetailModel[] {
                            new Models.DataTransfer.Receipt.ItemReceiptDetailModel
                            {
                                NewDetail = true,
                                Quantity = 1,
                                UnitId = null,
                                LineNumber = 1,
                                CreatedDate = DateTime.Now,
                                CreatedUserId = userId,
                                ItemId = dbMold.MoldItemId,
                                ReceiptStatus = (int)ReceiptStatusType.Created,
                                UnitPrice = 0,
                            }
                        },
                    });
                }

                // SAVE CHANGES FOR UPDATING MOLD ITEM RELATION
                if (moldItemSet)
                    _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }
        #endregion

        #region MOLD TEST BUSINESS
        public MoldTestModel[] GetMoldTestList()
        {
            MoldTestModel[] data = new MoldTestModel[0];

            var repo = _unitOfWork.GetRepository<MoldTest>();

            data = repo.GetAll().ToList()
                .Select(d => new MoldTestModel
                {
                    Id = d.Id,
                    CreatedDate = d.CreatedDate,
                    CreatedUserId = d.CreatedUserId,
                    DyeCode = d.DyeCode,
                    DyeId = d.DyeId,
                    HeadSize = d.HeadSize,
                    InflationTimeSeconds = d.InflationTimeSeconds,
                    InPackageQuantity = d.InPackageQuantity,
                    InPalletPackageQuantity = d.InPalletPackageQuantity,
                    MachineCode = d.Machine != null ? d.Machine.MachineCode : "",
                    MachineName = d.Machine != null ? d.Machine.MachineName : "",
                    TestDateStr = string.Format("{0:dd.MM.yyyy}", d.TestDate),
                    MachineId = d.MachineId,
                    MoldCode = d.MoldCode,
                    MoldId = d.MoldId,
                    MoldName = d.MoldName,
                    NutCaliber = d.NutCaliber,
                    NutQuantity = d.NutQuantity,
                    PackageDimension = d.PackageDimension,
                    PlantId = d.PlantId,
                    ProductCode = d.ProductCode,
                    ProductDescription = d.ProductDescription,
                    ProductId = d.ProductId,
                    ProductName = d.ProductName,
                    RalCode = d.RalCode,
                    RawItemName = d.RawMaterialName,
                    RawMaterialGr = d.RawMaterialGr,
                    RawMaterialGrText = d.RawMaterialGrText,
                    RawMaterialName = d.RawMaterialName,
                    RawMaterialId = d.RawMaterialId,
                    RawMaterialTolerationGr = d.RawMaterialTolerationGr,
                    TestDate = d.TestDate,
                    TotalTimeSeconds = d.TotalTimeSeconds,
                    UpdatedDate = d.UpdatedDate,
                    UpdatedUserId = d.UpdatedUserId,
                }).ToArray();

            return data.ToArray();
        }

        public MoldTestModel FindMoldTestByProduct(string productCode)
        {
            MoldTestModel model = new MoldTestModel();

            try
            {
                var repo = _unitOfWork.GetRepository<MoldTest>();
                var dbObj = repo.Get(d => d.ProductCode == productCode);
                if (dbObj != null)
                {
                    dbObj.MapTo(model);
                }
            }
            catch (Exception)
            {

            }

            return model;
        }

        public BusinessResult SaveOrUpdateMoldTest(MoldTestModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                if ((model.MachineId ?? 0) == 0)
                    throw new Exception("Makine seçilmelidir.");
                if (model.MoldId == null && string.IsNullOrEmpty(model.MoldCode))
                    throw new Exception("Kalıp bilgisi girilmelidir.");

                var repo = _unitOfWork.GetRepository<MoldTest>();
                var repoMold = _unitOfWork.GetRepository<Mold>();

                var dbObj = repo.Get(d => d.Id == model.Id);
                if (dbObj == null)
                {
                    dbObj = new MoldTest();
                    dbObj.CreatedDate = DateTime.Now;
                    dbObj.CreatedUserId = model.CreatedUserId;
                    repo.Add(dbObj);
                }

                var crDate = dbObj.CreatedDate;
                var tsDate = dbObj.TestDate;

                model.MapTo(dbObj);

                if (dbObj.CreatedDate == null)
                    dbObj.CreatedDate = crDate;
                if (dbObj.TestDate == null)
                    dbObj.TestDate = tsDate;

                #region GENERATE MOLD DEFINITION IF NOT EXISTS
                var dbMold = repoMold.Get(d => d.MoldCode == dbObj.MoldCode);
                if (dbMold == null && !string.IsNullOrEmpty(dbObj.MoldCode))
                {
                    dbMold = new Mold
                    {
                        IsActive=true,
                        MoldCode=dbObj.MoldCode,
                        MoldName = dbObj.MoldName,
                    };
                    repoMold.Add(dbMold);
                }
                #endregion

                dbObj.UpdatedDate = DateTime.Now;

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

        public BusinessResult DeleteMoldTest(int id)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<MoldTest>();

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

        public MoldTestModel GetMoldTest(int id)
        {
            MoldTestModel model = new MoldTestModel { };

            var repo = _unitOfWork.GetRepository<MoldTest>();
            var dbObj = repo.Get(d => d.Id == id);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);
                model.TestDateStr = string.Format("{0:dd.MM.yyyy}", model.TestDate);
            }

            return model;
        }

        #endregion
    }
}
