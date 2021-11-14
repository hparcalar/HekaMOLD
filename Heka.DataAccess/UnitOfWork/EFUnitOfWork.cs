using Heka.DataAccess.Context;
using Heka.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Heka.DataAccess.UnitOfWork;
using System.Data.Entity;

namespace Heka.DataAccess.UnitOfWork
{
    public class EFUnitOfWork : IUnitOfWork
    {
        private readonly DbContext _dbContext;

        public EFUnitOfWork()
        {
            _dbContext = new HekaEntities();
        }

        public EFUnitOfWork(DbContext dbContext)
        {
            if (dbContext == null)
                throw new ArgumentNullException("dbContext can not be null.");

            _dbContext = dbContext;
        }

        public void SetTimeout(int seconds)
        {
            _dbContext.Database.CommandTimeout = seconds;
        }

        #region IUnitOfWork Members
        public IRepository<T> GetRepository<T>() where T : class
        {
            return new EFRepository<T>(_dbContext);
        }

        public int SaveChanges()
        {
            try
            {
                // Transaction işlemleri burada ele alınabilir veya Identity Map kurumsal tasarım kalıbı kullanılarak
                // sadece değişen alanları güncellemeyide sağlayabiliriz.
                return _dbContext.SaveChanges();
            }
            catch
            {
                // Burada DbEntityValidationException hatalarını handle edebiliriz.
                throw;
            }
        }
        #endregion

        #region IDisposable Members
        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                }
            }

            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
