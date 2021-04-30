using App.Business.BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Business
{
    public class UnitOfWork : IDisposable
    {
        DbContext _dbContext;
        public UnitOfWork(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        private AppUserRepository _appUserRepository;
        public AppUserRepository AppUserRepository
        {
            get
            {
                if (_appUserRepository == null)
                    _appUserRepository = new AppUserRepository(_dbContext);
                return _appUserRepository;
            }
        }
        private MQMessageRepository _mQMessageRepository;
        public MQMessageRepository MQMessageRepository
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
