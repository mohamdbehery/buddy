using Buddy.Utilities;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQClientWinService.Helpers
{
    public interface IRabbitMQClient
    {
        IModel Channel { get; set; }
        void EstablishRabbitMQ();
        void PublishNewMessages(List<Message> messages);

        void ConsumeNewMessages();

        void PublishMessage(string queue, Message msg);
    }
}
