using System;
namespace Project.Entities
{
    public interface IEntity
    {

    }

    public abstract class BaseEntity<TKey> : IEntity
    {
        public TKey Id { get; set; }
    }

    public abstract class BaseEntity : BaseEntity<Guid>
    {

    }

    public abstract class BaseEntityWithDetails : BaseEntity<Guid>
    {
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset ModifiedAt { get; set; }
        public string CreatedByUserIp { get; set; }
        public string ModifiedByUserIp { get; set; }

    }
}
