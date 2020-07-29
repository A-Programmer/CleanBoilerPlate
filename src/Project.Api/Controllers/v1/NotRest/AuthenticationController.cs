using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Project.Api.Resources.UserDtos;
using Project.Core.Services.SecutiryServices;
using Project.Core.Services.UserServices;
using Project.Entities.SharedEntity;

namespace Project.Api.Controllers.v1
{
    [ApiVersion("1")]
    [AllowAnonymous]
    public class AuthenticationController : BaseController
    {
        private readonly IJwtService _service;
        private readonly IUserService _userService;
        public AuthenticationController(IJwtService service,
            IUserService userService) 
        {
            _service = service;
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost]
        public virtual async Task<ActionResult<AccessToken>> Login([FromForm]TokenRequestDto  login)
        {
            if (!login.grant_type.Equals("password", StringComparison.OrdinalIgnoreCase))
                throw new Exception("OAuth flow is not correct.");
             
            var user = _userService.FindByPhoneAsync(login.username)
                ?? await _userService.FindByEmailAsync(login.username)
                ?? await _userService.FindByNameAsync(login.username);


            if (user == null)
                return new NotFoundObjectResult("Wrong Username or password");


            var isValidUserData = await _userService.CheckPasswordAsync(user, login.password);
            if (!isValidUserData)
                return new NotFoundObjectResult("Wrong Username or password");


            var isActiveUser = await _userService.IsUserStatusConfirmedAsync(user);
            if (!isActiveUser)
                return new NotFoundObjectResult("User is not active");

            await _userService.UpdateSecurityStampAsync(user);
            var token = await _service.GenerateToken(user);
            return token;
        }



    }
}
