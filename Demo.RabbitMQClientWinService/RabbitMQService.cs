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
        MessageQueueClient mQClient;
        Helper helper = Helper.CreateInstance();
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
                int interval = 0;
                return int.TryParse(helper.GetAppKey("FetchMessagesTimeIntervalInMSs"), out interval) ? interval : 5000;
            }
        }

        public RabbitMQService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {

            helper.Logger.Log("RabbitMQ Service started, Execution mode: " + (UseThreadPool ? "Thread Pool" : "RabbitMQ"));
            if (UseThreadPool)
                mQClient = new ManualMQClient();
            else
                mQClient = new RabbitMQClient();

            try
            {
                helper.Logger.Log("Publisher started..");
                serviceTimer.Elapsed += (sender, e) =>
                {
                    helper.Logger.Log("...^_^...");
                    List<Message> messages = mQClient.FetchMQMessages();
                    if (messages.Count > 0)
                        mQClient.PublishNewMessages(messages);
                };
                serviceTimer.Interval = FetchMessagesTimeIntervalInMSs;
                serviceTimer.Enabled = true;
            }
            catch (Exception ex)
            {
                helper.Logger.Log($"Rabbit MQ Exception: {ex.ToString()}");
            }
        }

        protected override void OnStop()
        {
            helper.Logger.Log("Rabbit MQ Service stopped...");
        }
    }
}
