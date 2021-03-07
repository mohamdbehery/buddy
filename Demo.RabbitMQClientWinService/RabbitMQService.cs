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
        public bool UseThreadPool
        {
            get
            {
                return helper.GetAppKey("UseThreadPool") == "1" ? true : false;
            }
        }

        public RabbitMQService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            helper.Logger.Log("RabbitMQ Service started, Execution mode: " + (UseThreadPool ? "Thread Pool" : "RabbitMQ"));
            try
            {
                // placing the start code in a task so that i can give the control back to the service
                Task.Factory.StartNew(() =>
                {
                    if (UseThreadPool)
                        mQClient = new ManualMQClient();
                    else
                        mQClient = new RabbitMQClient();

                    mQClient.StartMessenger();
                });
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
