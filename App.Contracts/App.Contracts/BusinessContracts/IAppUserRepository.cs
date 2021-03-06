﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Contracts.BusinessContracts
{
    public interface IAppUserRepository<TEntityModel> where TEntityModel : class
    {
        void AddUser(TEntityModel entityModel);
        IEnumerable<TEntityModel> GetByCriteria(TEntityModel entityModel);
    }
}
