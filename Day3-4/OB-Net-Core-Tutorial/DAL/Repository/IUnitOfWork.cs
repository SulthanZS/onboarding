using Microsoft.EntityFrameworkCore.Storage;
using OB_Net_Core_Tutorial.DAL.Models;
using OB_Net_Core_Tutorial.DAL.Repository;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OB_Net_Core_Tutorial.DAL.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IBaseRepository<Car> CarRepository { get; }
        void Save();
        Task SaveAsync(CancellationToken cancellationToken = default(CancellationToken));
        IDbContextTransaction StartNewTransaction();
        Task<IDbContextTransaction> StartNewTransactionAsync();
        Task<int> ExecuteSqlCommandAsync(string sql, object[] parameters, CancellationToken cancellationToken = default(CancellationToken));
    }
}
