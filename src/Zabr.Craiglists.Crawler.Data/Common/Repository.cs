using Microsoft.EntityFrameworkCore;

namespace Zabr.Craiglists.Crawler.Data.Common
{
    public abstract class Repository<TEntity>
        where TEntity : BaseEntity<int>
    {
        public readonly CraiglistsContext DbContext;

        protected Repository(CraiglistsContext context)
        {
            DbContext = context;
        }

        public IQueryable<TEntity> GetAll()
        {
            return DbContext.Set<TEntity>().AsNoTracking();
        }

        public virtual async Task<TEntity?> GetByIdAsync(int id)
        {
            return await DbContext.Set<TEntity>()
                .AsNoTracking()
                .SingleOrDefaultAsync(e => e.Id == id);
        }

        public virtual void Add(TEntity entity)
        {
            DbContext.Set<TEntity>().Add(entity);
            DbContext.SaveChanges();
        }

        public virtual async Task AddAsync(TEntity entity)
        {
            await DbContext.Set<TEntity>().AddAsync(entity);
            await DbContext.SaveChangesAsync();
        }

        public virtual async Task UpdateAsync(int id, TEntity entity)
        {
            DbContext.Set<TEntity>().Update(entity);

            await DbContext.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            DbContext.Set<TEntity>().Remove(entity);

            await DbContext.SaveChangesAsync();
        }
    }
}
