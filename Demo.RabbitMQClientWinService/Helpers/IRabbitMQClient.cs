using Buddy.Utilities;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RabbitMQClientWinService.Helpers.Enums;

namespace RabbitMQClientWinService.Helpers
{
    public interface IRabbitMQClient
    {
        IModel RabbitMQChannel { get; set; }
        public string DefaultQueue { get; }
        public bool ParallelExecuteMessages { get; }
        void EstablishRabbitMQ();
        void PublishNewMessages(List<Message> messages);

        void PublishMessage(string queue, Message msg);
        void ConsumeNewMessages();
        void ConsumeMessage(BasicDeliverEventArgs e, Message message);
        void MessageAknowledge(BasicDeliverEventArgs e, RabbitMQMessageState state);
    }
}
