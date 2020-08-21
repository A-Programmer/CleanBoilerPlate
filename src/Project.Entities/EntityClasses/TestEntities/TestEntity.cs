using System;
namespace Project.Entities.EntityClasses.TestEntities
{
    public class TestEntity : BaseEntityWithDetails<Guid>
    {
        public TestEntity()
        {
            CreatedTime = DateTime.Now;
        }

        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}
