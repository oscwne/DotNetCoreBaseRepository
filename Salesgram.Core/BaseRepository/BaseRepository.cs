using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Salesgram.Core.BaseRepository
{
    public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        private readonly DbContext _context;
        public BaseRepository(DbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Generic async method for entities
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="navigationProperties"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter = null, Expression<Func<TEntity, object>>[] navigationProperties = null)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (navigationProperties != null)
            {
                foreach(var navigationProperty in navigationProperties)
                {
                    query = query.Include(navigationProperty);
                }
            }

            return await query.ToListAsync();
        }

        /// <summary>
        /// Generic async method based on id for entities
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<TEntity> GetByIdAsync(object id)
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }
    }
}
