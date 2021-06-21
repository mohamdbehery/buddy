using App.Contracts.Core;
using Buddy.Utilities;
using RabbitMQ.Client.Events;
using RabbitMQClientWinService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static RabbitMQClientWinService.Models.MQEnums;

namespace RabbitMQClientWinService.MessageQueue
{
    public class ManualMQClient : MessageQueueClient
    {
        readonly Helper helper = Helper.CreateInstance();
        readonly ILogger logger = Logger.GetInstance();
        private bool continueRun = true;
        public override int MessageCountToFetch
        {
            get
            {
                int temp = 1;
                return int.TryParse(helper.GetAppKey("ManualMQMessageCountToFetch"), out temp) ? temp : 1;
            }
        }

        public override void StartMessenger()
        {
            logger.Log("Publisher started..");
            try
            {
                OnMessengerStarted();
                do
                {
                    List<MQMessage> messages = FetchMQMessages();
                    if (messages.Any())
                        PublishNewMessages(messages);
                    else
                        Thread.Sleep(FetchMessagesTimeIntervalInMSs);
                }
                while (continueRun);
            }
            catch (Exception ex)
            {
                logger.Log($"Exception: {ex.ToString()}");
                continueRun = false;
            }
        }

        public override void PublishNewMessages(List<MQMessage> messages)
        {
            List<Task> taskList = new List<Task>();
            foreach (var message in messages)
            {
                if (ParallelExecuteMessages)
                    taskList.Add(Task.Run(() => { ConsumeMessage(null, message); }));
                else
                    ConsumeMessage(null, message);
            }
            if (taskList.Any())
            {
                Task.WaitAll(taskList.ToArray());
                Task.WhenAll(taskList).ContinueWith((res) =>
                {
                    logger.Log($"Done executing message by thread pool...");
                });
            }
            else
                logger.Log($"Done executing message by thread pool...");

        }

        public override void MessageAknowledge(MQMessageState state, BasicDeliverEventArgs e)
        {
            switch (state)
            {
                case MQMessageState.SuccessfullyProcessed:
                    logger.Log("Success remove from queue");
                    break;
                case MQMessageState.UnsuccessfulProcessing:
                    logger.Log("Unsuccessful, requeue and retry");
                    break;
                default:
                    logger.Log("Bad Message, Reject and Delete");
                    break;
            }
        }
    }
}
