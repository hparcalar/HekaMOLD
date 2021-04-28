using Heka.DataAccess.Context;
using Heka.DataAccess.UnitOfWork;
using HekaMOLD.Business.Helpers;
using HekaMOLD.Business.Models.DataTransfer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.Business.Base
{
    public abstract class IBusinessObject : IDisposable
    {
        protected IUnitOfWork _unitOfWork;
        protected bool _autoSave = true;

        public IBusinessObject()
        {
            _unitOfWork = new EFUnitOfWork();
        }

        public IBusinessObject(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public bool AutoSave
        {
            get { return _autoSave; }
            set { _autoSave = value; }
        }

        public void Dispose()
        {
            if (_unitOfWork != null)
                _unitOfWork.Dispose();

            GC.SuppressFinalize(this);
        }

        #region BASE LOGIC
        public PlantModel[] GetPlantList()
        {
            var repo = _unitOfWork.GetRepository<Plant>();
            List<PlantModel> list = new List<PlantModel>();
            var dataList = repo.GetAll();
            dataList.ToList().ForEach(d => { list.Add(d.MapTo(new PlantModel())); });

            return list.ToArray();
        }
        #endregion
    }
}
