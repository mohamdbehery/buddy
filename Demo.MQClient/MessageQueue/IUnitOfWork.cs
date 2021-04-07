using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQClient.MessageQueue
{
    public interface IUnitOfWork : IDisposable
    {
        public MessageRepository Messages { get;}
        int Complete();
    }
}
