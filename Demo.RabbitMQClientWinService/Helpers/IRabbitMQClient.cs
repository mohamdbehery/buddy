using Buddy.Utilities;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQClientWinService.Helpers
{
    public interface IRabbitMQClient
    {
        IModel RabbitMQChannel { get; set; }
        void EstablishRabbitMQ();
        void PublishNewMessages(List<Message> messages);

        void PublishMessage(string queue, Message msg);
        void ConsumeNewMessages();
        void ConsumeMessageAsync(BasicDeliverEventArgs e);
        void MessageAknowledge(BasicDeliverEventArgs e, ReceivedMessageState state);
    }
}
