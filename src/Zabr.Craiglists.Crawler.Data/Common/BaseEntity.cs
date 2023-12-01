namespace Zabr.Craiglists.Crawler.Data.Common;

public abstract class BaseEntity<TEntityId>
{
    public TEntityId Id { get; init; }

    protected BaseEntity(TEntityId id)
    {
        Id = id;
    }
}
