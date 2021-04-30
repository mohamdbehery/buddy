using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Contracts.BusinessContracts
{
    public interface IMQMessageRepository<TEntityModel> where TEntityModel : class
    {
        IEnumerable<TEntityModel> FetchNewMessages(int count);
        IEnumerable<TEntityModel> GetByCriteria(TEntityModel entityModel);
    }
}
