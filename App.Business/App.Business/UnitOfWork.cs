using App.Business.BusinessObjects;
using App.Contracts;
using App.Contracts.BusinessContracts;
using App.Data.EFCore;
using App.Data.LogicalModelsDTO;
using Microsoft.EntityFrameworkCore;
using System;

namespace App.Business
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        readonly DbContext _dbContext;

        public UnitOfWork()
        {
            _dbContext = new BuddyDBContext();
        }

        private AppUserRepository _appUserRepository;
        public IAppUserRepository<AppUserModel> AppUserRepository
        {
            get
            {
                if (_appUserRepository == null)
                    _appUserRepository = new AppUserRepository(_dbContext);
                return _appUserRepository;
            }
        }

        private MQMessageRepository _mQMessageRepository;
        public IMQMessageRepository<DemoMQMessageModel> MQMessageRepository
        {
            get
            {
                if (_mQMessageRepository == null)
                    _mQMessageRepository = new MQMessageRepository(_dbContext);
                return _mQMessageRepository;
            }
        }
        public int Save()
        {
            return _dbContext.SaveChanges();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
