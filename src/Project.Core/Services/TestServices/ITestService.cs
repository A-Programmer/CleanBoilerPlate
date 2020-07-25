using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Project.Common;
using Project.Entities.EntityClasses.TestEntities;

namespace Project.Core.Services.TestServices
{
    public interface ITestService
    {
        Task<ResultMessage> Create(TestEntity testEntity);
        Task<ResultMessage> Remove(string id);
        Task<IEnumerable<TestEntity>> GetAll();
        Task<TestEntity> GetById(string id);
    }
}
