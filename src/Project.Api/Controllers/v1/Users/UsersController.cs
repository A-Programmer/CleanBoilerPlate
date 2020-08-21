using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Project.Api.Resources.UserDtos;
using Project.Api.ViewModels.IdentityViewModels;
using Project.Common.Utilities;
using Project.Core.Services.TestServices;
using Project.Core.Services.UserServices;
using Project.Entities;
using Project.Entities.Common;
using Project.Entities.EntityClasses.IdentityEntities;
using Project.Webframeworks;
using Project.Webframeworks.Api;

namespace Project.Api.Controllers.v1
{
    [ApiVersion("1")]
    public class UsersController : BaseController
    {
        private readonly IMyEntityService _es;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IRoleService _roleService;
        private readonly IConfiguration _configuration;
        private readonly PublicOptions _options;
        public UsersController(
            IUserService userService,
            IMapper mapper,
            IRoleService roleService,
            IConfiguration configuration,
            IMyEntityService es
        )
        {
            _userService = userService;
            _mapper = mapper;
            _roleService = roleService;
            _configuration = configuration;
            _options = configuration.GetSection("PublicSettings:PublicOptions").Get<PublicOptions>();
            _es = es;
        }


        #region Get users
        [HttpGet]
        public virtual async Task<ActionResult<IEnumerable<UsersListResponseDto>>> Get(int skip = 0, int take = 0)
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





        [Route("/api/v{version:apiVersion}/entities")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<MyEntity>> Post(EntityDto dto)
        {
            var entity = new MyEntity();
            entity.MyName = dto.MyFirstName;
            entity.MyLastName = dto.MyLastName;

            var result = await _es.Create(entity);
            return result;
        }

        [Route("/api/v{version:apiVersion}/entities/{id}")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<MyEntity>> Get(int id)
        {
            var result = await _es.GetById(id);
            return result;
        }

        [Route(Routes.MyEntity.RootAddress)]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IEnumerable<MyEntity>> GetAll()
        {
            var result = await _es.GetAll();
            return result;
        }



    }

    public class EntityDto
    {
        public string MyFirstName { get; set; }
        public string MyLastName { get; set; }
    }
}