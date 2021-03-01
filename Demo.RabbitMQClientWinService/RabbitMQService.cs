using Buddy.Utilities;
using RabbitMQ.Client;
using RabbitMQClientWinService.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace RabbitMQClientWinService
{
    partial class RabbitMQService : ServiceBase
    {
        IMessageQueueClient rabbitMQClient;
        Helper helper = Helper.CreateInstance();
        DBHelper dbHelper = Helper.CreateInstance<DBHelper>();
        Timer serviceTimer = Helper.CreateInstance<Timer>();
        public bool UseThreadPool
        {
            get
            {
                return helper.GetAppKey("UseThreadPool") == "1" ? true : false;
            }
        }

        public int FetchMessagesTimeIntervalInMSs
        {
            get
            {
                int defaultInterval = 5000;
                var temp = helper.GetAppKey("FetchMessagesTimeIntervalInMSs");
                if(!string.IsNullOrEmpty(temp))
                {
                    int interval = 0;
                    return int.TryParse(temp, out interval)? interval: defaultInterval;
                }
                return defaultInterval;
            }
        }

        public RabbitMQService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {

            helper.Log("RabbitMQ Service started, Execution mode: " + (UseThreadPool ? "Thread Pool" : "RabbitMQ"));
            rabbitMQClient = Helper.CreateInstance<RabbitMQClient>();
            try
            {
                helper.Log("Publisher started..");
                serviceTimer.Elapsed += (sender, e) =>
                {
                    helper.Log("...^_^...");
                    List<Message> messages = dbHelper.FetchMQMessages();
                    if (messages.Count > 0)
                    {
                        if (UseThreadPool)
                            PublishMessagesToThreadPool(messages);
                        else
                            rabbitMQClient.PublishNewMessages(messages);
                    }
                };
                serviceTimer.Interval = FetchMessagesTimeIntervalInMSs;
                serviceTimer.Enabled = true;
                if (!UseThreadPool)
                    rabbitMQClient.ConsumeNewMessages();
            }
            catch (Exception ex)
            {
                helper.Log($"Rabbit MQ Exception: {ex.ToString()}");
            }
        }

        private void PublishMessagesToThreadPool(List<Message> messages)
        {
            List<Task> taskList = new List<Task>();
            foreach (var message in messages)
            {
                taskList.Add(Task.Run(() => { rabbitMQClient.ConsumeMessage(null, message); }));
            }
            Task.WaitAll(taskList.ToArray());
            Task.WhenAll(taskList).ContinueWith((res) =>
            {
                helper.Log($"Done executing messages in thread pool...");
            });
        }

        protected override void OnStop()
        {
            helper.Log("Rabbit MQ Service stopped...");
        }
    }
}
