using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Project.Common;
using Project.Entities;

namespace Project.Core.Services.TestServices
{
    public interface IMyEntityService
    {
        Task<MyEntity> Create(MyEntity testEntity);
        Task<IEnumerable<MyEntity>> GetAll();
        Task<MyEntity> GetById(int id);
    }
}
