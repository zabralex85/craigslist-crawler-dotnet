using Zabr.Craiglists.Crawler.Data.Common;
using Zabr.Craiglists.Crawler.Data.Entities;

namespace Zabr.Craiglists.Crawler.Data.Repositories
{
    public class PageRepository : Repository<Page>, IPageRepository
    {
        public PageRepository(CraiglistsContext context)
            : base(context) 
        {
        }
    }
}
