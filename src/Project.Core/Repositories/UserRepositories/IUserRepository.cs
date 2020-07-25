using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Project.Entities;

namespace Project.Core.Repositories.UserRepositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByLoginInfo(string username, string passwordHash);
        Task<IEnumerable<User>> GetPagedUsers(int skip, int take);
    }
}
