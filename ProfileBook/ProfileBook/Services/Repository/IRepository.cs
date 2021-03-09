using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ProfileBook.Models;

namespace ProfileBook.Services.Repository
{
    public interface IRepository
    {
        Task<List<T>> GetAllAsync<T>()
        where T : class, IEntityBase, new();

        Task<T> GetSingleByIdAsync<T>(long id)
        where T : class, IEntityBase, new();

        Task<T> GetSingleAsync<T>(Expression<Func<T, bool>> predicate)
        where T : class, IEntityBase, new();

        Task<List<T>> FindByAsync<T>(Expression<Func<T, bool>> predicate)
        where T : class, IEntityBase, new();

        Task<int> DeleteAsync<T>(T entity)
        where T : class, IEntityBase, new();

        Task DeleteWhereAsync<T>(Expression<Func<T, bool>> predicate)
        where T : class, IEntityBase, new();

        Task DeleteAllAsync<T>()
        where T : class, IEntityBase, new();

        Task SaveOrUpdateRangeAsync<T>(IEnumerable<T> entities)
        where T : class, IEntityBase, new();

        Task<int> CountAsync<T>()
        where T : class, IEntityBase, new();

        Task<int> CountByAsync<T>(Expression<Func<T, bool>> predicate)
        where T : class, IEntityBase, new();

        Task<int> SaveOrUpdateAsync<T>(T entity)
        where T : class, IEntityBase, new();

        Task<int> SaveAsync<T>(T entity)
        where T : class, IEntityBase, new();
    }
}
