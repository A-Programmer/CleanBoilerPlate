using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Project.Common;
using Project.Core;
using Project.Core.Services.TestServices;
using Project.Entities.EntityClasses.TestEntities;

namespace Project.Services.TestServices
{
    public class TestService : ITestService
    {
        private readonly IUnitOfWork _uow;
        public TestService(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public async Task<ResultMessage> Create(TestEntity testEntity)
        {
            var rm = new ResultMessage()
            {
                Status = false,
                Message = ""
            };
            try
            {
                await _uow.TestRepository.AddAsync(testEntity);
                await _uow.CommitAsync();
                rm.Status = true;
                rm.Message = "Succeeded";
            }
            catch(Exception ex)
            {
                rm.Status = false;
                rm.Message = ex.Message;
            }
            return rm;
        }

        public async Task<IEnumerable<TestEntity>> GetAll()
        {
            return await _uow.TestRepository.GetAllAsync();
        }

        public async Task<TestEntity> GetById(string id)
        {
            return await _uow.TestRepository.GetByIdAsync(id);
        }

        public async Task<ResultMessage> Remove(string id)
        {
            var rm = new ResultMessage()
            {
                Status = false,
                Message = ""
            };
            try
            {
                var entity = await _uow.TestRepository.GetByIdAsync(id);
                if(entity != null)
                {
                    _uow.TestRepository.Remove(entity);
                    await _uow.CommitAsync();
                    rm.Status = true;
                    rm.Message = "Succeeded";
                }
                else
                {
                    rm.Status = false;
                    rm.Message = "Record not found";
                }
            }
            catch (Exception ex)
            {
                rm.Status = false;
                rm.Message = ex.Message;
            }
            return rm;
        }
    }
}
