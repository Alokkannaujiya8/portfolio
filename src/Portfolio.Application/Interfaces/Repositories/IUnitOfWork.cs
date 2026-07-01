using System;
using System.Threading;
using System.Threading.Tasks;

namespace Portfolio.Application.Interfaces.Repositories;

public interface IUnitOfWork : IDisposable
{
    IRepository<T> Repository<T>() where T : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
