using Microsoft.EntityFrameworkCore;
using Zabr.Crawler.Data.Common;
using Zabr.Crawler.Data.Entities;

namespace Zabr.Crawler.Data.Repositories
{
    public class PageRepository : Repository<PageEntity>, IPageRepository
    {
        public PageRepository(DataDbContext context)
            : base(context) 
        {
        }

        public async Task AddIfNoExistsAsync(PageEntity entity)
        {
            var exists = await base.ExistAsync(x => x.Url == entity.Url);
            if (!exists)
            {
                await base.AddAsync(entity);
            }
        }

        public async Task<string[]?> GetAllUrls()
        {
            return await this.DbContext.Pages.Select(x => x.Url).ToArrayAsync();
        }
    }
}
