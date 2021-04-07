using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Contracts
{
    public interface IAppUserRepository<TEntityModel> where TEntityModel : class
    {
        List<TEntityModel> CustomGetAll(TEntityModel entityModel);
    }
}
