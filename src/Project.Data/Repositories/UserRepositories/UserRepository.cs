using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Project.Common.Utilities;
using Project.Core.Repositories.UserRepositories;
using Project.Entities;

namespace Project.Data.Repositories.UserRepositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext db)
            : base(db)
        {
        }


        public async Task<User> GetByLoginInfo(string username, string passwordHash)
        {
            return await DbContext.Users
                .FirstOrDefaultAsync(x => x.UserName == username && x.PasswordHash == passwordHash);
        }

        public async Task<IEnumerable<User>> GetPagedUsers(int skip, int take)
        {
            return await DbContext.Users
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }


        private ApplicationDbContext DbContext
        {
            get
            {
                return Context as ApplicationDbContext;
            }
        }
    }
}
