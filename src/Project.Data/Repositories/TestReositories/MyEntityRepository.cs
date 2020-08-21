using System;
using Project.Core.Repositories.TestRepositories;
using Project.Entities;

namespace Project.Data.Repositories.TestReositories
{
    public class MyEntityRepository : Repository<MyEntity>, IMyEntityRepository
    {
        public MyEntityRepository(ApplicationDbContext db)
            : base(db)
        {
        }
    }
}
