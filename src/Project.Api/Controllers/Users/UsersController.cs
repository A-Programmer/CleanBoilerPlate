using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.Api.Resources.UserDtos;
using Project.Core.Services.UserServices;
using Project.Entities;


namespace Project.Api.Controllers.Users
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IRoleService _roleService;
        public UsersController(
            IUserService userService,
            IMapper mapper,
            IRoleService roleService
        )
        {
            _userService = userService;
            _mapper = mapper;
            _roleService = roleService;
        }

        #region Get users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsersListResponseDto>>> List(int skip = 0, int take = 0)
        {
            var users = Enumerable.Empty<User>();
            if (take > 0)
            {
                users = await _userService.GetUsers(skip, take);
            }
            else
            {
                users = await _userService.GetUsers();
            }
            var result = _mapper.Map<List<UsersListResponseDto>>(users);
            return result;
        }
        #endregion

        #region Get user by id
        [HttpGet("{id}")]
        public async Task<ActionResult<UserProfileDto>> GetById(string id)
        {
            var guidId = Guid.Parse(id);
            var user = await _userService.GetUser(guidId);
            if (user == null)
                return new NotFoundObjectResult("User not found");
            var userDto = _mapper.Map<UserProfileDto>(user);
            return userDto;
        }
        #endregion

        #region Register
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<UserProfileDto>> Register(UserRegisterDto userDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var user = _mapper.Map<User>(userDto);
            var result = await _userService.RegisterAsync(user, userDto.Password);
            if(result.Status)
            {
                await _roleService.AddUserToRoleAsync(user, "user");
                var registeredUser = await _userService.FindByNameAsync(userDto.UserName);
                var userProfileDto = _mapper.Map<UserProfileDto>(registeredUser);
                return userProfileDto;
            }
            else
            {
                return new BadRequestObjectResult(result.Message);
            }
        }
        #endregion

        #region Update User

        #endregion


    }
}
