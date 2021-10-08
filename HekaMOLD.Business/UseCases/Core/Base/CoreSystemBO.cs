using Heka.DataAccess.Context;
using HekaMOLD.Business.Base;
using HekaMOLD.Business.Helpers;
using HekaMOLD.Business.Models.Constants;
using HekaMOLD.Business.Models.DataTransfer.Core;
using HekaMOLD.Business.Models.Operational;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.UseCases.Core.Base
{
    public class CoreSystemBO : IBusinessObject
    {
        #region PRINTER DEFINITION BUSINESS
        public SystemPrinterModel[] GetPrinterList()
        {
            List<SystemPrinterModel> data = new List<SystemPrinterModel>();

            var repo = _unitOfWork.GetRepository<SystemPrinter>();

            repo.GetAll().ToList().ForEach(d =>
            {
                SystemPrinterModel containerObj = new SystemPrinterModel();
                d.MapTo(containerObj);
                data.Add(containerObj);
            });

            return data.ToArray();
        }

        public BusinessResult SaveOrUpdatePrinter(SystemPrinterModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                if (string.IsNullOrEmpty(model.PrinterCode))
                    throw new Exception("Yazıcı kodu girilmelidir.");

                var repo = _unitOfWork.GetRepository<SystemPrinter>();

                if (repo.Any(d => (d.PrinterCode == model.PrinterCode && d.PlantId == model.PlantId)
                    && d.Id != model.Id))
                    throw new Exception("Aynı koda sahip başka bir yazıcı mevcuttur. Lütfen farklı bir kod giriniz.");

                var dbObj = repo.Get(d => d.Id == model.Id);
                if (dbObj == null)
                {
                    dbObj = new SystemPrinter();
                    repo.Add(dbObj);
                }

                model.MapTo(dbObj);

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

        public BusinessResult DeletePrinter(int id)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<SystemPrinter>();

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

        public SystemPrinterModel GetPrinter(int id)
        {
            SystemPrinterModel model = new SystemPrinterModel { };

            var repo = _unitOfWork.GetRepository<SystemPrinter>();
            var dbObj = repo.Get(d => d.Id == id);
            if (dbObj != null)
            {
                model = dbObj.MapTo(model);
            }

            return model;
        }
        #endregion

        #region ALLOCATED CODE MANAGEMENT FOR SYSTEM GENERATED CODES
        public BusinessResult AllocateCode(int recordType)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<AllocatedCode>();
                string lastAllocated = repo.Filter(d => d.ObjectType == recordType)
                    .OrderByDescending(d => d.AllocatedCode1)
                    .Select(d => d.AllocatedCode1)
                    .FirstOrDefault();

                if (recordType == (int)RecordType.SerialItem)
                {
                    if (string.IsNullOrEmpty(lastAllocated))
                    {
                        var repoSerial = _unitOfWork.GetRepository<WorkOrderSerial>();
                        string lastSerialNo = repoSerial.GetAll()
                            .OrderByDescending(d => d.SerialNo)
                            .Select(d => d.SerialNo)
                            .FirstOrDefault();

                        if (string.IsNullOrEmpty(lastSerialNo))
                            lastSerialNo = "0";

                        string lastCode = string.Format("{0:00000000}", Convert.ToInt32(lastSerialNo) + 1);
                        repo.Add(new AllocatedCode
                        {
                            ObjectType = recordType,
                            CreatedDate = DateTime.Now,
                            AllocatedCode1 = lastCode,
                        });
                        result.Code = lastCode;
                    }
                    else
                    {
                        string lastCode = string.Format("{0:00000000}", Convert.ToInt32(lastAllocated) + 1);
                        repo.Add(new AllocatedCode
                        {
                            ObjectType = recordType,
                            CreatedDate = DateTime.Now,
                            AllocatedCode1 = lastCode,
                        });
                        result.Code = lastCode;
                    }
                }

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
        #endregion

        #region PRINTING QUEUE BUSINESS
        public BusinessResult AddToPrintQueue(PrinterQueueModel model)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<PrinterQueue>();
                var dbObj = new PrinterQueue
                {
                    CreatedDate = DateTime.Now,
                    OrderNo = null,
                    RecordId = model.RecordId,
                    PrinterId = model.PrinterId,
                    RecordType = model.RecordType,
                    AllocatedPrintData = model.AllocatedPrintData,
                };
                repo.Add(dbObj);

                _unitOfWork.SaveChanges();
                result.RecordId = dbObj.Id;
                result.Result = true;
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public PrinterQueueModel GetNextFromPrinterQueue(int printerId)
        {
            var repo = _unitOfWork.GetRepository<PrinterQueue>();
            return repo.Filter(d => d.PrinterId == printerId)
                .OrderBy(d => d.CreatedDate)
                .Select(d => new PrinterQueueModel
                {
                    Id = d.Id,
                    CreatedDate = d.CreatedDate,
                    OrderNo = d.OrderNo,
                    PrinterId = d.PrinterId,
                    RecordId = d.RecordId,
                    AllocatedPrintData = d.AllocatedPrintData,
                    RecordType = d.RecordType,
                }).FirstOrDefault();
        }

        public BusinessResult SetElementAsPrinted(int printerQueueId)
        {
            BusinessResult result = new BusinessResult();

            try
            {
                var repo = _unitOfWork.GetRepository<PrinterQueue>();
                var dbObj = repo.Get(d => d.Id == printerQueueId);
                if (dbObj != null)
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
        #endregion
    }
}
