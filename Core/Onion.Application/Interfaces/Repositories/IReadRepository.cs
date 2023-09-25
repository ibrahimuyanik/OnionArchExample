using Microsoft.EntityFrameworkCore.Query;
using Onion.Domain.Common;
using System.Linq.Expressions;

namespace Onion.Application.Interfaces.Repositories
{
    public interface IReadRepository<T> where T : class, IEntityBase, new()
    {
        Task<IList<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null, 
           Func<IQueryable<T>,IIncludableQueryable<T, object>>? include = null, // 2.parametre ilgili entity'nin then include işlemi yapabilmek için
                     Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, // bu parametre entity'deki belirttiğimiz property'ye göre sıralama işlemi yapmak için
                                                   bool enableTracking = false);

        Task<IList<T>> GetAllByPagingAsync(Expression<Func<T, bool>>? predicate = null,
                  Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null, 
                             Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, 
                   bool enableTracking = false, int currentPage = 1, int pageSize = 3);

        Task<T> GetAsync(Expression<Func<T, bool>> predicate,
                         Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null, 
                         bool enableTracking = false);

        IQueryable<T> Find(Expression<Func<T, bool>> predicate, bool enableTracking = false);

        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
    }
}
