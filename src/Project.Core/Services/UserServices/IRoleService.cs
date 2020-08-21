using System;
using System.Threading.Tasks;
using Project.Common;
using Project.Entities;
using Project.Entities.EntityClasses.IdentityEntities;

namespace Project.Core.Services.UserServices
{
    public interface IRoleService
    {
        Task<ResultMessage> AddUserToRoleAsync(User user, string roleName);
    }
}
