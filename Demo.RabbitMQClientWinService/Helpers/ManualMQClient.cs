using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static RabbitMQClientWinService.Helpers.Enums;

namespace RabbitMQClientWinService.Helpers
{
    public class ManualMQClient : MessageQueueClient
    {
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
            helper.Logger.Log("Publisher started..");
            try
            {
                OnMessengerStarted();
                do
                {
                    List<Message> messages = FetchMQMessages();
                    if (messages.Any())
                        PublishNewMessages(messages);
                    else
                        Thread.Sleep(FetchMessagesTimeIntervalInMSs);
                }
                while (continueRun);
            }
            catch (Exception ex)
            {
                helper.Logger.Log($"Exception: {ex.ToString()}");
                continueRun = false;
            }
        }

        public override void PublishNewMessages(List<Message> messages)
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
                    helper.Logger.Log($"Done executing message by thread pool...");
                });
            }
            else
                helper.Logger.Log($"Done executing message by thread pool...");

        }

        public override void MessageAknowledge(MQMessageState state, BasicDeliverEventArgs e)
        {
            switch (state)
            {
                case MQMessageState.SuccessfullyProcessed:
                    helper.Logger.Log("Success remove from queue");
                    break;
                case MQMessageState.UnsuccessfulProcessing:
                    helper.Logger.Log("Unsuccessful, requeue and retry");
                    break;
                default:
                    helper.Logger.Log("Bad Message, Reject and Delete");
                    break;
            }
        }
    }
}
