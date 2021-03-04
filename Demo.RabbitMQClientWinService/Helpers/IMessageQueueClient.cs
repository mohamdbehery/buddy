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
    public interface IMessageQueueClient
    {
        string DefaultQueue { get; }
        int MessageCountToFetch { get; }
        bool ParallelExecuteMessages { get; }
        public int FetchMessagesTimeIntervalInMSs { get; }
        void StartMessenger();
        void PublishNewMessages(List<Message> messages);
        void ConsumeMessage(BasicDeliverEventArgs e, Message message);
        void MessageAknowledge(MQMessageState state, BasicDeliverEventArgs e = null);
    }
}
