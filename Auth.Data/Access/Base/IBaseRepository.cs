using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Auth.Data.Access.Base
{
    public interface IBaseRepository<TEntity> where TEntity: class
    {
        TEntity Get(string id);
        Task<TEntity> GetAsync(string id);

        TEntity FindOne(Expression<Func<TEntity, bool>> match);
        Task<TEntity> FindOneAsync(Expression<Func<TEntity, bool>> match);

        IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> match = null);
        Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> match = null);

        //IEnumerable<TEntity> Search(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null);

        //Task<IEnumerable<TEntity>> SearchAsync(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null);

        //int Count(Expression<Func<TEntity, bool>> match = null);
        //Task<int> CountAsync(Expression<Func<TEntity, bool>> match = null);
        
        // WRITE operations
        TEntity Add(TEntity t);

        TEntity Update(TEntity updated, long key);

        TEntity Delete(TEntity t);
    }
}
