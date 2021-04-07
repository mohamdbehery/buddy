using App.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Data.EFCore.ConceptualModels;
using App.Data.EFCore;
using Buddy.Utilities;

namespace App.Business
{
    public class AppUserBL
    {
        Helper helper = Helper.CreateInstance();
        readonly BuddyDBContext db = new BuddyDBContext();
        public List<AppUser> GetUsers(AppUserModel appUserModel)
        {
            AppUser appUser = helper.MapObjects<AppUserModel, AppUser>(appUserModel);

            var xx = db.AppUsers.Select(x => x);
            List<AppUser> users;
            var query = db.AppUsers;
            if (appUserModel.Id > 0)
                users = query.Where(x => x.Id == appUserModel.Id).ToList();
            else
                users = query.Where(x => string.IsNullOrEmpty(appUserModel.eMailAddress) || x.eMailAddress == appUserModel.eMailAddress
                && string.IsNullOrEmpty(appUserModel.Password) || x.Password == appUserModel.Password
                ).ToList();

            return users;
        }

        public int SaveUser(AppUserModel userModel)
        {
            AppUser row = helper.MapObjects<AppUserModel, AppUser>(userModel);
            row.EntryDate = DateTime.Now;
            row.IsDeleted = false;
            row.ModifyDate = DateTime.Now;
            
            if (userModel.Id > 0)
            {
                var existRow = db.AppUsers.Find(userModel.Id);
                // TODO update changes from input model to this row
                existRow.ModifyDate = DateTime.Now;
            }
            else
                db.AppUsers.Add(row);

            db.SaveChanges();
            return row.Id;
        }
    }
}
