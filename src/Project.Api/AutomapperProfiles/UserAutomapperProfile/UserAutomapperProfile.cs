using System;
using System.Linq;
using AutoMapper;
using Project.Api.Resources.UserDtos;
using Project.Common.Utilities;
using Project.Entities;
using Project.Entities.EntityClasses.IdentityEntities;

namespace Project.Api.AutomapperProfiles.UserAutomapperProfile
{
    public class UserAutomapperProfile : Profile
    {
        public UserAutomapperProfile()
        {
            CreateMap<User, UserRegisterDto>()
                .ForMember(x => x.Password, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<User, UsersListResponseDto>()
                .ReverseMap();

            CreateMap<User, UserProfileDto>()
                .ReverseMap();

            CreateMap<User, UpdateUserDto>()
                .ReverseMap();


        }
    }

}
