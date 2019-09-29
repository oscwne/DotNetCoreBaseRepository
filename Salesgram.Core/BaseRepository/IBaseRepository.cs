using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Salesgram.Core.BaseRepository
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        DbSet<TEntity> DbSet { get; }
        Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter = null, Expression<Func<TEntity, object>>[] navigationProperties = null);
        Task<TEntity> GetByIdAsync(object id);
    }
}
