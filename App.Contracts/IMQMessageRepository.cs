using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Contracts
{
    public interface IMQMessageRepository<TEntity> where TEntity : class
    {
        List<TEntity> FetchNewMessages(int count);
    }
}
