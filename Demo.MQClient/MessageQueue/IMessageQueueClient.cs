using Buddy.Utilities;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQClientWinService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RabbitMQClientWinService.Models.MQEnums;

namespace RabbitMQClientWinService.MessageQueue
{
    public interface IMessageQueueClient
    {
        string DefaultQueue { get; }
        int MessageCountToFetch { get; }
        bool ParallelExecuteMessages { get; }
        public int FetchMessagesTimeIntervalInMSs { get; }
        void StartMessenger();
        void PublishNewMessages(List<MQMessage> messages);
        void ConsumeMessage(BasicDeliverEventArgs e, MQMessage message);
        void MessageAknowledge(MQMessageState state, BasicDeliverEventArgs e = null);
    }
}
