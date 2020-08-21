using System;
using System.Threading.Tasks;
using Project.Core;
using Project.Core.Repositories.TestRepositories;
using Project.Core.Repositories.UserRepositories;
using Project.Data.Repositories.TestReositories;
using Project.Data.Repositories.UserRepositories;

namespace Project.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        private UserRepository _users;
        private TestRepository _testRepository;
        private MyEntityRepository _entityRepository;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public ITestRepository TestRepository => _testRepository = _testRepository ?? new TestRepository(_context);
        public IUserRepository Users => _users = _users ?? new UserRepository(_context);

        public IMyEntityRepository MyEntityRepository => _entityRepository ?? new MyEntityRepository(_context);

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
