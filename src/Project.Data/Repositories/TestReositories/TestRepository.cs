using System;
using Project.Core.Repositories.TestRepositories;
using Project.Entities.EntityClasses.TestEntities;

namespace Project.Data.Repositories.TestReositories
{
    public class TestRepository : Repository<TestEntity>, ITestRepository
    {
        public TestRepository(ApplicationDbContext db)
            : base(db)
        {
        }
    }
}
