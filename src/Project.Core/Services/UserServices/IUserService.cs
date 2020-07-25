using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Project.Common;
using Project.Entities;

namespace Project.Core.Services.UserServices
{
    public interface IUserService
    {
        Task<User> FindByNameAsync(string username);
        Task<User> FindByEmailAsync(string email);
        User FindByPhoneAsync(string phone);
        Task<bool> CheckPasswordAsync(User user, string password);
        Task<bool> IsUserConfirmed(
            User user,
            bool checkByStatus = false,
            bool checkByEmail = false,
            bool checkByPhoneNumber = false
            );
        Task<bool> IsUserStatusConfirmedAsync(User user);
        Task<bool> IsPhoneNumberConfirmedAsync(User user);
        Task<bool> IsEmailConfirmedAsync(User user);
        Task<IEnumerable<User>> GetUsers(int skip, int take);



        Task<ResultMessage> RegisterAsync(User user, string password);
        Task<string> GenerateToken(User user);
        Task<ResultMessage> ChangePassword(User user, string token, string newPassword);
        Task UpdateSecurityStampAsync(User user);


        Task<User> GetUser(Guid id);
        Task<IEnumerable<User>> GetUsers();
        Task UpdateLoginDate(User user);
        Task<bool> HasValidSecurityStampAsync(ClaimsPrincipal principal);
    }
}
