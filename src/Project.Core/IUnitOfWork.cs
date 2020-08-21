using System;
using System.Threading.Tasks;
using Project.Core.Repositories.TestRepositories;
using Project.Core.Repositories.UserRepositories;

namespace Project.Core
{
    public interface IUnitOfWork : IDisposable
    {
        ITestRepository TestRepository { get; }
        IUserRepository Users { get; }
        IMyEntityRepository MyEntityRepository { get;  }
        Task<int> CommitAsync();
    }
}
