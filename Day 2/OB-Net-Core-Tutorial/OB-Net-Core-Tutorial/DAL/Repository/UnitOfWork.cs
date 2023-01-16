using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using OB_Net_Core_Tutorial.DAL.Data;
using OB_Net_Core_Tutorial.DAL.Models;
using OB_Net_Core_Tutorial.DAL.Repository;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OB_Net_Core_Tutorial.DAL.Repository
{
    public class UnitOfWork : IDisposable
    {
        private readonly OB_Net_Core_TutorialContext dbContext;

        public CarRepository CarRepository { get; }
        public TypeRepository TypeRepository { get; }

        public UnitOfWork(OB_Net_Core_TutorialContext context)
        {
            dbContext = context;

            CarRepository = new CarRepository(context);
            TypeRepository = new TypeRepository(context);
        }

        public void Save()
        {
            dbContext.SaveChanges();
        }

        public Task SaveAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return dbContext.SaveChangesAsync(cancellationToken);
        }

        public IDbContextTransaction StartNewTransaction()
        {
            return dbContext.Database.BeginTransaction();
        }

        public Task<IDbContextTransaction> StartNewTransactionAsync()
        {
            return dbContext.Database.BeginTransactionAsync();
        }

        public Task<int> ExecuteSqlCommandAsync(string sql, object[] parameters, CancellationToken cancellationToken = default(CancellationToken))
        {
            return dbContext.Database.ExecuteSqlRawAsync(sql, parameters, cancellationToken);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    dbContext?.Dispose();
                }
                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}
