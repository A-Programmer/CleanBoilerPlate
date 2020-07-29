using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Project.Common;
using Project.Common.Utilities;
using Project.Core;
using Project.Core.Services.UserServices;
using Project.Entities;

namespace Project.Services.UserServices
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _uow;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public UserService(IUnitOfWork uow,
            SignInManager<User> signInManager,
            UserManager<User> userManager)
        {
            _uow = uow;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        #region Queries
        public async Task<User> FindByNameAsync(string username)
        {
            return await _userManager.FindByNameAsync(username);
        }

        public async Task<User> FindByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public User FindByPhoneAsync(string phone)
        {
            return _uow.Users.Find(x => x.PhoneNumber != null && x.PhoneNumber == phone).FirstOrDefault();
        }

        public async Task<bool> CheckPasswordAsync(User user, string password)
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task UpdateLoginDate(User user)
        {
            var existingUser = await GetUser(user.Id);

            if (existingUser != null)
            {
                existingUser.LastLoginDate = DateTime.UtcNow;
                await _uow.CommitAsync();
            }
        }

        public async Task<bool> IsUserConfirmed(
            User user,
            bool checkByStatus = false,
            bool checkByEmail = false,
            bool checkByPhoneNumber = false
            )
        {
            var statusResult = true;
            var emailResult = true;
            var phoneResult = true;
            if (checkByStatus)
                statusResult = await IsUserStatusConfirmedAsync(user);
            if (checkByEmail)
                emailResult = await IsEmailConfirmedAsync(user);
            if (checkByPhoneNumber)
                phoneResult = await IsPhoneNumberConfirmedAsync(user);

            return (statusResult && emailResult && phoneResult);
        }

        public async Task<bool> IsUserStatusConfirmedAsync(User user)
        {
            return (await _uow.Users.SingleOrDefaultAsync(x => x.Id == user.Id && x.IsActive) != null);
        }

        public async Task<bool> IsPhoneNumberConfirmedAsync(User user)
        {
            return await _userManager.IsPhoneNumberConfirmedAsync(user);
        }

        public async Task<bool> IsEmailConfirmedAsync(User user)
        {
            return await _userManager.IsEmailConfirmedAsync(user);
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            return await _uow.Users.GetAllAsync();
        }

        public async Task<IEnumerable<User>> GetUsers(int skip, int take)
        {
            return await _uow.Users.GetPagedUsers(skip, take);
        }

        #endregion


        #region Commands
        public async Task UpdateSecurityStampAsync(User user)
        {
            await _userManager.UpdateSecurityStampAsync(user);
        }
        public async Task<ResultMessage> RegisterAsync(User user, string password)
        {
            var rm = new ResultMessage()
            {
                Status = false,
                Message = ""
            };
            var existancePhoneNumber = FindByPhoneAsync(user.PhoneNumber);
            if(existancePhoneNumber != null)
            {
                rm.Status = false;
                rm.Message = "A user with this phone number exist";
                return rm;
            }
            user.RegisteredAt = DateTime.Now;
            if (user.UserName.IsValidEmail())
                user.Email = user.UserName;
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await UpdateLoginDate(user);
                rm.Status = true;
                rm.Message = "Registration succeeded";
            }
            else
            {
                rm.Status = false;
                foreach (var error in result.Errors)
                {
                    rm.Message += error.Description + "\n";
                }
            }
            return rm;
        }

        public async Task<string> GenerateToken(User user)
        {
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<ResultMessage> ChangePassword(User user,string token, string newPassword)
        {
            var rm = new ResultMessage()
            {
                Status = false,
                Message = ""
            };
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            if (result.Succeeded)
            {
                rm.Status = true;
                rm.Message = "Password has been changed";
            }
            else
            {
                rm.Status = false;
                foreach (var error in result.Errors)
                {
                    rm.Message += error.Description + "\n";
                }
            }
            return rm;
        }
        public async Task<ResultMessage> RemoveUser(User user)
        {
            var rm = new ResultMessage()
            {
                Status = false,
                Message = ""
            };
            try
            {
                var roles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, roles);
                var identityResult = await _userManager.DeleteAsync(user);
                if(identityResult.Succeeded)
                {
                    rm.Status = true;
                    rm.Message = "User has been removed.";
                }
                else
                {
                    rm.Status = false;
                    foreach(var error in identityResult.Errors)
                    {
                        rm.Message += error.Description + "\n";
                    }
                }
            }
            catch(Exception ex)
            {
                rm.Status = false;
                rm.Message = ex.Message;
            }
            return rm;
        }
        public async Task<ResultMessage> Update(User user)
        {
            var rm = new ResultMessage
            {
                Status = false,
                Message = ""
            };
            var existingUser = await _uow.Users.GetByIdAsync(user.Id);
            if (existingUser == null)
            {
                rm.Status = false;
                rm.Message = "User not found";
                return rm;
            }
            
            var identityResult = await _userManager.UpdateAsync(user);
            if(identityResult.Succeeded)
            {
                rm.Status = true;
                rm.Message = "User has been updated.";
            }
            else
            {
                rm.Status = false;
                foreach(var error in identityResult.Errors)
                {
                    rm.Message += error.Description + "\n";
                }
            }
            return rm;
        }
        public async Task<ResultMessage> UpdateVerificationCode(Guid id, int verificationCode)
        {
            var rm = new ResultMessage()
            {
                Status = false,
                Message = ""
            };

            var user = await _uow.Users.GetByIdAsync(id);
            if (user != null)
            {
                user.VerificationCode = verificationCode;
                await _uow.CommitAsync();
                rm.Status = true;
                rm.Message = "OTP Code generated.";
            }
            else
            {
                rm.Status = false;
                rm.Message = "User could not be found.";
            }
            return rm;
        }
        public async Task<ResultMessage> UpdatePhoneVerificationStatus(Guid id, bool newStatus)
        {
            var rm = new ResultMessage()
            {
                Status = false,
                Message = ""
            };

            var user = await _uow.Users.GetByIdAsync(id);
            if (user != null)
            {
                user.PhoneNumberConfirmed = newStatus;
                await _uow.CommitAsync();
                rm.Status = true;
                rm.Message = "PhoneNumber has been confirmed.";
            }
            else
            {
                rm.Status = false;
                rm.Message = "User could not be found.";
            }
            return rm;
        }
        public async Task<ResultMessage> UpdateEmailVerificationStatus(Guid id, bool newStatus)
        {
            var rm = new ResultMessage()
            {
                Status = false,
                Message = ""
            };

            var user = await _uow.Users.GetByIdAsync(id);
            if (user != null)
            {
                user.EmailConfirmed = newStatus;
                await _uow.CommitAsync();
                rm.Status = true;
                rm.Message = "Email address has been confirmed.";
            }
            else
            {
                rm.Status = false;
                rm.Message = "User could not be found.";
            }
            return rm;
        }
        #endregion






        public async Task<User> GetUser(Guid id)
        {
            return await _uow.Users.GetByIdAsync(id);
        }


        public async Task<bool> HasValidSecurityStampAsync(ClaimsPrincipal principal)
        {
            var user = await _signInManager.ValidateSecurityStampAsync(principal);
            return (user != null);
        }

        
    }
}
