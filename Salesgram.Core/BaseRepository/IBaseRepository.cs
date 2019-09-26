using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Salesgram.Core.BaseRepository
{
    interface IBaseRepository<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter = null, Expression<Func<TEntity, object>>[] navigationProperties = null);
        Task<TEntity> GetByIdAsync(object id);
    }
}
