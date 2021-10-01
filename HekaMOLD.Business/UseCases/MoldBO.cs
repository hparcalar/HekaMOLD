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
                var repoReceipt = _unitOfWork.GetRepository<ItemReceiptDetail>();

                var dbMold = repoMold.Get(d => d.Id == moldId);
                data = repoReceipt.Filter(d => d.ItemId == dbMold.MoldItemId).ToList()
                    .Select(d => new MoldMoveHistory
                    {
                        ItemReceiptDetailId = d.Id,
                        ItemReceiptId = d.ItemReceiptId,
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
                        ReceiptDocumentNo = d.ItemReceipt != null ? d.ItemReceipt.DocumentNo : "",
                    }).ToArray();
            }
            catch (Exception)
            {

            }

            return data;
        }
        #endregion

        #region MOLD TEST BUSINESS
        public MoldTestModel[] GetMoldTestList()
        {
            List<MoldTestModel> data = new List<MoldTestModel>();

            var repo = _unitOfWork.GetRepository<MoldTest>();

            repo.GetAll().ToList().ForEach(d =>
            {
                MoldTestModel containerObj = new MoldTestModel();
                d.MapTo(containerObj);
                containerObj.MachineCode = d.Machine != null ? d.Machine.MachineCode : "";
                containerObj.MachineName = d.Machine != null ? d.Machine.MachineName : "";
                containerObj.TestDateStr = string.Format("{0:dd.MM.yyyy}", d.TestDate);
                data.Add(containerObj);
            });

            return data.ToArray();
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
