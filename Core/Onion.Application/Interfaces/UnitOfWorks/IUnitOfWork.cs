using Onion.Application.Interfaces.Repositories;
using Onion.Domain.Common;

namespace Onion.Application.Interfaces.UnitOfWorks
{
    public interface IUnitOfWork: IAsyncDisposable
    {
        IReadRepository<T> GetReadRepository<T>() where T : class, IEntityBase, new();
        IWriteRepository<T> GetWriteRepository<T>() where T : class, IEntityBase, new();
        Task<int> SaveAsync();
        int Save();

    }
}
