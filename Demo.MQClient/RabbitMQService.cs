using App.Contracts.Core;
using Buddy.Utilities;
using RabbitMQ.Client;
using RabbitMQClientWinService.MessageQueue;
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
        readonly ILogger logger = Logger.GetInstance();
        public bool UseThreadPool
        {
            get
            {
                return helper.GetAppKey("UseThreadPool") == "1";
            }
        }

        public RabbitMQService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            logger.Log("RabbitMQ Service started, Execution mode: " + (UseThreadPool ? "Thread Pool" : "RabbitMQ"));
            try
            {
                // the below mQClient is the publisher which has the DelegateEvent implemented
                // subscriper to DelegateEvent implemented in MessageQueueClient
                Notifier notifier = new Notifier();
                if (UseThreadPool)
                    mQClient = new ManualMQClient();
                else
                    mQClient = new RabbitMQClient();

                // before StartMessanger, subscripe with notifier
                mQClient.MessengerStarted += notifier.OnMessengerStarted;

                // placing the start code in a task so that i can give the control back to the service
                Task.Factory.StartNew(() => { mQClient.StartMessenger(); });
            }
            catch (Exception ex)
            {
                logger.Log($"Rabbit MQ Exception: {ex.ToString()}");
            }
        }

        protected override void OnStop()
        {
            logger.Log("Rabbit MQ Service stopped...");
        }
    }
}
