using Heka.DataAccess.Context;
using HekaMOLD.Business.Base;
using HekaMOLD.Business.Helpers;
using HekaMOLD.Business.Models.DataTransfer.Production;
using HekaMOLD.Business.UseCases.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.UseCases.Core
{
    public class CoreProductionBO : CoreReceiptsBO
    {
        public string GetNextProductRecipeNo()
        {
            try
            {
                var repo = _unitOfWork.GetRepository<ProductRecipe>();
                string lastReceiptNo = repo.GetAll()
                    .OrderByDescending(d => d.ProductRecipeCode)
                    .Select(d => d.ProductRecipeCode)
                    .FirstOrDefault();

                if (string.IsNullOrEmpty(lastReceiptNo))
                    lastReceiptNo = "0";

                return string.Format("{0:000000}", Convert.ToInt32(lastReceiptNo) + 1);
            }
            catch (Exception)
            {

            }

            return default;
        }

        public string GetNextWorkOrderNo()
        {
            try
            {
                var repo = _unitOfWork.GetRepository<WorkOrder>();
                string lastWorkOrderNo = repo.GetAll()
                    .OrderByDescending(d => d.WorkOrderNo)
                    .Select(d => d.WorkOrderNo)
                    .FirstOrDefault();

                if (string.IsNullOrEmpty(lastWorkOrderNo))
                    lastWorkOrderNo = "0";

                return string.Format("{0:000000}", Convert.ToInt32(lastWorkOrderNo) + 1);
            }
            catch (Exception)
            {

            }

            return default;
        }

        public string GetNextSerialNo()
        {
            try
            {
                var repo = _unitOfWork.GetRepository<WorkOrderSerial>();
                string lastSerialNo = repo.GetAll()
                    .OrderByDescending(d => d.SerialNo)
                    .Select(d => d.SerialNo)
                    .FirstOrDefault();

                if (string.IsNullOrEmpty(lastSerialNo))
                    lastSerialNo = "0";

                return string.Format("{0:00000000}", Convert.ToInt64(lastSerialNo) + 1);
            }
            catch (Exception)
            {

            }

            return default;
        }

        public string GetNextPalletNo()
        {
            try
            {
                var repo = _unitOfWork.GetRepository<Pallet>();
                string lastSerialNo = repo.GetAll()
                    .OrderByDescending(d => d.PalletNo)
                    .Select(d => d.PalletNo)
                    .FirstOrDefault();

                if (string.IsNullOrEmpty(lastSerialNo))
                    lastSerialNo = "0";

                return string.Format("{0:00000000}", Convert.ToInt64(lastSerialNo) + 1);
            }
            catch (Exception)
            {

            }

            return default;
        }

        public ShiftModel GetCurrentShift()
        {
            ShiftModel data = new ShiftModel();

            try
            {
                var repoShift = _unitOfWork.GetRepository<Shift>();

                // RESOLVE CURRENT SHIFT
                DateTime entryTime = DateTime.Now;
                DateTime shiftBelongsTo = DateTime.Now.Date;

                Shift dbShift = null;
                var shiftList = repoShift.Filter(d => d.StartTime != null && d.EndTime != null).ToArray();
                foreach (var shift in shiftList)
                {
                    DateTime startTime = DateTime.Now.Date.Add(shift.StartTime.Value);
                    DateTime endTime = DateTime.Now.Date.Add(shift.EndTime.Value);

                    if (shift.StartTime > shift.EndTime)
                    {
                        if (DateTime.Now.Hour >= shift.StartTime.Value.Hours) // gece 12 den önce
                        {
                            endTime = DateTime.Now.Date.AddDays(1).Add(shift.EndTime.Value);
                            shiftBelongsTo = DateTime.Now.Date;
                        }
                        else // gece 12den sonra
                        {
                            startTime = DateTime.Now.Date.AddDays(-1).Add(shift.StartTime.Value);
                            shiftBelongsTo = DateTime.Now.Date.AddDays(-1);
                        }
                    }
                    else
                        shiftBelongsTo = DateTime.Now.Date;

                    if (entryTime >= startTime && entryTime <= endTime)
                    {
                        dbShift = shift;
                        break;
                    }
                }

                if (dbShift != null)
                {
                    dbShift.MapTo(data);
                    data.ShiftBelongsToDate = shiftBelongsTo;
                    data.ChiefName = dbShift.User != null ? dbShift.User.UserName : "";
                }
            }
            catch (Exception)
            {

            }

            return data;
        }
    }
}
