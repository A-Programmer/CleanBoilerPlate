using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Project.Api.Resources.UserDtos;
using Project.Core.Services.SecutiryServices;
using Project.Core.Services.UserServices;


namespace Project.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IJwtService _service;
        private readonly IUserService _userService;
        public AuthenticationController(IJwtService service,
            IUserService userService) 
        {
            _service = service;
            _userService = userService;
        }

        [HttpPost]
        public async Task<ActionResult<string>> Login(UserLoginDto login)
        {
            var user = _userService.FindByPhoneAsync(login.UserName)
                ?? await _userService.FindByEmailAsync(login.UserName)
                ?? await _userService.FindByNameAsync(login.UserName);


            if (user == null)
                return new NotFoundObjectResult("Wrong Username or password");


            var isValidUserData = await _userService.CheckPasswordAsync(user, login.Password);
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
