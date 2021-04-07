using App.Data.EFCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQClient.MessageQueue
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext _dBContext;
        public UnitOfWork(DbContext dBContext)
        {
            _dBContext = dBContext;
            Messages = new MessageRepository(_dBContext);
        }
        public MessageRepository Messages { get; private set; }

        public int Complete()
        {
            return _dBContext.SaveChanges();
        }

        public void Dispose()
        {
            _dBContext.Dispose();
        }
    }
}
