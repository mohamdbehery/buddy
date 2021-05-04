using App.Contracts.BusinessContracts;
using App.Contracts.Core;
using App.Data.EFCore;
using App.Data.EFCore.ConceptualModels;
using App.Data.LogicalModelsDTO;
using Buddy.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace App.Business.BusinessObjects
{
    public class AppUserRepository : Repository<AppUser>, IAppUserRepository<AppUserModel>
    {
        readonly Helper helper = Helper.CreateInstance();
        DbContext _context;

        public AppUserRepository(DbContext dbContext) : base(dbContext)
        {
            _context = dbContext;
        }

        public void AddUser(AppUserModel entityModel)
        {
            AppUser row; 
            if (entityModel.Id > 0)
            {
                row = Find(user => user.Id == entityModel.Id).FirstOrDefault();
                row.ModifyDate = DateTime.Now;
                row.eMailAddress = entityModel.eMailAddress;
                row.FamilyName = entityModel.FamilyName;
                row.FirstName = entityModel.FirstName;
                row.IsActive = entityModel.IsActive;
                row.IsDeleted = entityModel.IsDeleted;
                row.LocationAddress = entityModel.LocationAddress;
                row.Password = entityModel.Password;
                row.SecondName = entityModel.SecondName;
                row.UserName = entityModel.UserName;
            }
            else
            {
                row = helper.MapObjects<AppUserModel, AppUser>(entityModel);
                row.EntryDate = DateTime.Now;
                row.IsDeleted = false;
                Add(row);
            }

            // TODO use the UnitOfWork
            //_context.SaveChanges();
            //entityModel.Id = row.Id;
            //return entityModel;
        }

        public IEnumerable<AppUserModel> GetByCriteria(AppUserModel entityModel)
        {
            List<AppUser> users;
            if (entityModel.Id > 0)
                users = Find(x => x.Id == entityModel.Id).ToList();
            else
                users = Find(x => string.IsNullOrEmpty(entityModel.eMailAddress) || x.eMailAddress == entityModel.eMailAddress
                && string.IsNullOrEmpty(entityModel.Password) || x.Password == entityModel.Password
                ).ToList();

            return helper.MapObjects<AppUser, AppUserModel>(users);
        }
    }
}
