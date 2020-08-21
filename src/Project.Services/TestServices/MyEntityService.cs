using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Project.Common;
using Project.Core;
using Project.Core.Services.TestServices;
using Project.Entities;

namespace Project.Services.TestServices
{
    public class MyEntityService : IMyEntityService
    {
        private readonly IUnitOfWork _uow;
        public MyEntityService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<MyEntity> Create(MyEntity testEntity)
        {
            await _uow.MyEntityRepository.AddAsync(testEntity);
            await _uow.CommitAsync();
            return testEntity;
        }

        public async Task<IEnumerable<MyEntity>> GetAll()
        {
            return await _uow.MyEntityRepository.GetAllAsync();
        }

        public async Task<MyEntity> GetById(int id)
        {
            return await _uow.MyEntityRepository.GetByIdAsync(id);
        }
    }
}
