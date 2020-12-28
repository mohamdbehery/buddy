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
        RabbitMQHelper rabbitMQ = new RabbitMQHelper();
        Helper helper = new Helper();
        Timer serviceTimer = new Timer();
        public RabbitMQService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                helper.Log("RabbitMQ Service started...");
                var channel = rabbitMQ.EstablishRabbitMQ();
                if (channel != null)
                {
                    helper.Log("Publisher started..");
                    serviceTimer.Elapsed += (sender, e) => EstablishRabbitMQPublisher(sender, e, channel);
                    serviceTimer.Interval = 5000;
                    serviceTimer.Enabled = true;

                    QueueConsumer.ConsumeNewMessages(channel);
                }
            }
            catch (Exception ex)
            {
                helper.Log($"Rabbit MQ Exception: {ex.ToString()}");
            }
        }

        private void EstablishRabbitMQPublisher(object source, ElapsedEventArgs e, IModel channel)
        {
            helper.Log("... ^_^ ...");
            QueuePublisher.PublishNewMessages(channel);
        }

        protected override void OnStop()
        {
            helper.Log("Rabbit MQ Service stopped...");
        }
    }
}
