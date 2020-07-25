using System;
using AutoMapper;
using Project.Api.Resources;
using Project.Entities.EntityClasses.TestEntities;

namespace Project.Api.AutomapperProfiles
{
    public class TestProfile : Profile
    {
        public TestProfile()
        {
            CreateMap<TestEntity, TestDto>().ReverseMap();
        }
    }
}
