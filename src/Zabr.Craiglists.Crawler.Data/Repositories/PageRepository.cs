using Zabr.Craiglists.Crawler.Data.Common;
using Zabr.Craiglists.Crawler.Data.Entities;

namespace Zabr.Craiglists.Crawler.Data.Repositories
{
    public class PageRepository : Repository<PageEntity>, IPageRepository
    {
        public PageRepository(CraiglistsContext context)
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
    }
}
