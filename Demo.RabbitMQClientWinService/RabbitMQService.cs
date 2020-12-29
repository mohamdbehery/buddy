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
using System.Timers;

namespace RabbitMQClientWinService
{
    partial class RabbitMQService : ServiceBase
    {
        RabbitMQClient rabbitMQClient;
        Helper helper = new Helper();
        DBHelper dbHelper = new DBHelper();

        Timer serviceTimer = new Timer();
        public RabbitMQService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            rabbitMQClient = new RabbitMQClient();
            try
            {
                helper.Log("RabbitMQ Service started...");
                helper.Log("Publisher started..");
                serviceTimer.Elapsed += (sender, e) =>
                {
                    helper.Log("... ^_^ ...");
                    List<Message> messages = dbHelper.FetchMQMessages();
                    if (messages.Count > 0)
                    {
                        rabbitMQClient.PublishNewMessages(messages);
                    }
                };
                serviceTimer.Interval = 5000;
                serviceTimer.Enabled = true;

                rabbitMQClient.ConsumeNewMessages();
            }
            catch (Exception ex)
            {
                helper.Log($"Rabbit MQ Exception: {ex.ToString()}");
            }
        }

        protected override void OnStop()
        {
            helper.Log("Rabbit MQ Service stopped...");
        }
    }
}
