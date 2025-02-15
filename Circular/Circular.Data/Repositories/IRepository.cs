using Circular.Core.Entity;
using System.Linq.Expressions;

namespace Circular.Data.Repositories
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<IEnumerable<T>?> GetAllAsync();
        Task<T?> GetAsync(long id);
        Task<IEnumerable<T>?> GetAsync(Expression<Func<T, bool>> where);
        Task<int> CreateAsync(T entity);
        Task<int> UpdateAsync(T entity);

        Task<int> DeleteAsync(T entity);

    }
}
