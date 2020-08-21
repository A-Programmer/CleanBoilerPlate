using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Project.Common;
using Project.Core;
using Project.Core.Services.UserServices;
using Project.Entities;
using Project.Entities.EntityClasses.IdentityEntities;

namespace Project.Services.UserServices
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _uow;
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;
        public RoleService(IUnitOfWork uow,
            RoleManager<Role> roleManager,
            UserManager<User> userManager
        )
        {
            _uow = uow;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<ResultMessage> AddUserToRoleAsync(User user, string roleName)
        {
            var rm = new ResultMessage()
            {
                Status = false,
                Message = ""
            };
            var roleExistance = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExistance)
                await _roleManager.CreateAsync(new Role()
                {
                    Name = roleName
                });
            var result = await _userManager.AddToRoleAsync(user, roleName);
            if(result.Succeeded)
            {
                rm.Status = true;
                rm.Message = "User added to role";
            }
            else
            {
                rm.Status = false;
                foreach(var error in result.Errors)
                {
                    rm.Message += error.Description + "\n";
                }
            }
            return rm;
        }
    }
}
