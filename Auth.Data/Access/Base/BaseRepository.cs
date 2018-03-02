using Auth.Data.Context;
using AutoMapper.Execution;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Auth.Data.Access.Base
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        protected IdentityContext _context;
        protected DbSet<TEntity> _dbEntitySet;

        public BaseRepository(IdentityContext context)
        {
            if (context == null)
            {
                throw new ArgumentException("An instance of DbContext is required to use this repository.", "context");
            }

            _context = context;
            _dbEntitySet = _context.Set<TEntity>();
        }     

        public async Task<TEntity> GetAsync(string id)
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }

        public TEntity Get(string id)
        {
            return _context.Set<TEntity>().FindAsync(id).Result;
        }
      

        public TEntity FindOne(Expression<Func<TEntity, bool>> match)
        {
            return _context.Set<TEntity>().SingleOrDefault(match);
        }

        public async Task<TEntity> FindOneAsync(Expression<Func<TEntity, bool>> match)
        {
            return await _context.Set<TEntity>().SingleOrDefaultAsync(match);
        }

        public IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> match = null)
        {
            if (match != null)
            {
                return _context.Set<TEntity>().Where(match).ToList();
            }
            return _context.Set<TEntity>().ToList();
        }

        public async Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> match = null)
        {
            if (match != null)
            {
                return await _context.Set<TEntity>().Where(match).ToListAsync();
            }
            return await _context.Set<TEntity>().ToListAsync();
        }

        //public IEnumerable<TEntity> Search(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null)
        //{            
        //    var results = _dbEntitySet.Where(queryExpression);

        //    return orderBy != null ? orderBy(results).ToList() : results.ToList();
        //}

        //public async Task<IEnumerable<TEntity>> SearchAsync(IList<Filter> filters,
        //    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null)
        //{
        //    var queryExpression = ExpressionBuilder.ExpressionBuilder.GetExpression<TEntity>(SanitizeFilters(filters));
        //    var results = queryExpression == null ? _dbEntitySet : _dbEntitySet.Where(queryExpression);

        //    return orderBy != null
        //        ? await orderBy(results).ToListAsync()
        //        : await results.ToListAsync();
        //}
        
      
        //public async Task<int> FilteredCountAsync(IList<Filter> filters)
        //{
        //    var queryExpression = ExpressionBuilder.ExpressionBuilder.GetExpression<TEntity>(SanitizeFilters(filters));
        //    return queryExpression == null ? await _context.Set<TEntity>().CountAsync()
        //        : await _context.Set<TEntity>().CountAsync(queryExpression);
        //}


        public TEntity Add(TEntity t)
        {
            return _context.Set<TEntity>().Add(t).Entity;
        }

        public TEntity Update(TEntity updated, long key)
        {
            if (updated == null)
                return null;

            var existing = _context.Set<TEntity>().Find(key);
            if (existing != null)
            {
                _context.Entry(existing).CurrentValues.SetValues(updated);
            }
            return existing;
        }

        public TEntity Delete(TEntity t)
        {
            return _context.Set<TEntity>().Remove(t).Entity;
        }
    }
}
