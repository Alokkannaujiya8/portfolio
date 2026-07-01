using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Portfolio.Application.Interfaces.Repositories;
using Portfolio.Infrastructure.Persistence.Context;
using Portfolio.Infrastructure.Persistence.Repositories;

namespace Portfolio.Infrastructure.Persistence.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly PortfolioDbContext _context;
    private readonly ConcurrentDictionary<string, object> _repositories;
    private bool _disposed;

    public UnitOfWork(PortfolioDbContext context)
    {
        _context = context;
        _repositories = new ConcurrentDictionary<string, object>();
    }

    public IRepository<T> Repository<T>() where T : class
    {
        var type = typeof(T).Name;

        return (IRepository<T>)_repositories.GetOrAdd(type, _ => new Repository<T>(_context));
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            _disposed = true;
        }
    }
}
