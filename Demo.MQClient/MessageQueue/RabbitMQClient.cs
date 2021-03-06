﻿using App.Contracts.Core;
using Buddy.Utilities;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQClientWinService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using static RabbitMQClientWinService.Models.MQEnums;

namespace RabbitMQClientWinService.MessageQueue
{
    public class RabbitMQClient : MessageQueueClient
    {
        Timer serviceTimer = Helper.CreateInstance<Timer>();
        readonly ILogger logger = Logger.GetInstance();
        readonly Helper helper = Helper.CreateInstance();
        public RabbitMQClient()
        {
        }

        public override int MessageCountToFetch
        {
            get
            {
                int temp = 5;
                return int.TryParse(helper.GetAppKey("RabbitMQMessageCountToFetch"), out temp) ? temp : 5;
            }
        }
        public IModel RabbitMQChannel { get; set; }
        public override void StartMessenger()
        {
            logger.Log("Publisher started..");
            serviceTimer.Elapsed += (sender, e) =>
            {
                logger.Log("...^_^...");
                List<MQMessage> messages = FetchMQMessages();
                if (messages.Count > 0)
                    PublishNewMessages(messages);
            };
            serviceTimer.Interval = FetchMessagesTimeIntervalInMSs;
            serviceTimer.Enabled = true;

            OnMessengerStarted();
        }

        public void EstablishRabbitMQ()
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    Uri = new Uri(helper.GetAppKey("RabbitMQUri"))
                };
                var connection = factory.CreateConnection();
                RabbitMQChannel = connection.CreateModel();
            }
            catch (Exception ex)
            {
                logger.Log($"Rabbit MQ Exception: {ex.ToString()}");
            }
        }

        public override void PublishNewMessages(List<MQMessage> messages)
        {
            try
            {
                this.EstablishRabbitMQ();
                RabbitMQChannel.QueueDeclare(DefaultQueue, durable: true, exclusive: false, autoDelete: false, arguments: null);
                logger.Log($"/////////////////////// Publishing {messages.Count} messages /////////////////////////////");
                foreach (MQMessage msg in messages)
                {
                    PublishMessage(DefaultQueue, msg);
                }
                logger.Log($"Done publishing messages...");

                ConsumeNewMessages();
            }
            catch (Exception ex)
            {
                logger.Log($"Exception: {ex.ToString()}");
            }
        }

        public void PublishMessage(string queue, MQMessage msg)
        {
            try
            {
                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(msg));
                string exchangeType = ""; // "" = default exchange
                var msgProps = RabbitMQChannel.CreateBasicProperties();
                msgProps.DeliveryMode = 1; // Non presistent message
                msgProps.Persistent = false;
                msgProps.Expiration = "36000000"; // in millisecond
                msgProps.MessageId = msg.MessageID.ToString();
                RabbitMQChannel.BasicPublish(exchangeType, queue, msgProps, body);
            }
            catch (Exception ex)
            {
                RecordMessageFailure(msg, $"Failed to publish: {ex.ToString()}");
            }
        }

        public void ConsumeNewMessages()
        {
            logger.Log("Consumer started..");
            this.EstablishRabbitMQ();
            RabbitMQChannel.QueueDeclare(DefaultQueue, durable: true, exclusive: false, autoDelete: false, arguments: null);
            var consumer = new EventingBasicConsumer(RabbitMQChannel);
            consumer.Received += (sender, e) =>
            {
                ConsumeMessage(e);
            };
            RabbitMQChannel.BasicConsume(DefaultQueue, false, consumer);
        }

        public override void MessageAknowledge(MQMessageState state, BasicDeliverEventArgs e)
        {
            switch (state)
            {
                case MQMessageState.SuccessfullyProcessed:
                    // Success remove from queue
                    logger.Log("Success remove from queue");
                    RabbitMQChannel.BasicAck(e.DeliveryTag, false);
                    break;
                case MQMessageState.UnsuccessfulProcessing:
                    // Unsuccessful, requeue and retry
                    logger.Log("Unsuccessful, requeue and retry");
                    RabbitMQChannel.BasicNack(e.DeliveryTag, false, true);
                    break;
                default:
                    // Bad Message, Reject and Delete
                    logger.Log("Bad Message, Reject and Delete");
                    RabbitMQChannel.BasicReject(e.DeliveryTag, false);
                    break;
            }
        }
    }
}
