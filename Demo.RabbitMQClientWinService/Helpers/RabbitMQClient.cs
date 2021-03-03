﻿using Buddy.Utilities;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RabbitMQClientWinService.Helpers.Enums;

namespace RabbitMQClientWinService.Helpers
{
    public class RabbitMQClient : IMessageQueueClient
    {
        DBHelper dbHelper;
        Helper helper;
        public RabbitMQClient()
        {
            dbHelper = Helper.CreateInstance<DBHelper>();
            helper = Helper.CreateInstance();
        }
        public IModel RabbitMQChannel { get; set; }
        public string DefaultQueue
        {
            get
            {
                string temp = helper.GetAppKey("RabbitMQDefaultQueue");
                return string.IsNullOrEmpty(temp) ? "mbehery" : temp;
            }
        }
        public bool ParallelExecuteMessages
        {
            get
            {
                return helper.GetAppKey("ParallelExecuteMessages") == "1" ? true : false;
            }
        }
        public bool UseThreadPool
        {
            get
            {
                return helper.GetAppKey("UseThreadPool") == "1" ? true : false;
            }
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
                helper.Logger.Log($"Rabbit MQ Exception: {ex.ToString()}");
            }
        }

        public void PublishNewMessages(List<Message> messages)
        {
            try
            {
                this.EstablishRabbitMQ();
                RabbitMQChannel.QueueDeclare(DefaultQueue, durable: true, exclusive: false, autoDelete: false, arguments: null);
                helper.Logger.Log($"/////////////////////// Publishing {messages.Count} messages /////////////////////////////");
                foreach (Message msg in messages)
                {
                    PublishMessage(DefaultQueue, msg);
                }
                helper.Logger.Log($"Done publishing messages...");
            }
            catch (Exception ex)
            {
                helper.Logger.Log($"Exception: {ex.ToString()}");
            }
        }

        public void PublishMessage(string queue, Message msg)
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
                dbHelper.RecordMessageFailure(msg, $"Failed to publish: {ex.ToString()}");
            }
        }

        public void ConsumeNewMessages()
        {
            helper.Logger.Log("Consumer started..");
            this.EstablishRabbitMQ();
            RabbitMQChannel.QueueDeclare(DefaultQueue, durable: true, exclusive: false, autoDelete: false, arguments: null);
            var consumer = new EventingBasicConsumer(RabbitMQChannel);
            consumer.Received += (sender, e) =>
            {
                ConsumeMessage(e);
            };
            RabbitMQChannel.BasicConsume(DefaultQueue, false, consumer);
        }

        public void ConsumeMessage(BasicDeliverEventArgs e, Message message = null)
        {
            helper.Logger.Log($"///////////// Message Received /////////////");
            if (message == null)
            {
                var body = e.Body.ToArray();
                var messageJson = Encoding.UTF8.GetString(body);
                message = JsonConvert.DeserializeObject<Message>(messageJson);
            }
            if (string.IsNullOrEmpty(message.MessageData) || message.MessageData == "INVALID")
            {
                dbHelper.RecordMessageFailure(message, $"Invalid Message: message id ({message.MessageID}) has invalid data!");
                MessageAknowledge(e, RabbitMQMessageState.MessageRejected);
            }
            else
            {
                if (!UseThreadPool && ParallelExecuteMessages)
                {
                    Task.Factory.StartNew(() => { return dbHelper.ExecuteMQMessage(message); }).ContinueWith((taskExec) =>
                    {
                        AfterMessageExecution(e, message, taskExec.Result);
                    });
                }
                else
                {
                    ExecResult returnedData = dbHelper.ExecuteMQMessage(message);
                    AfterMessageExecution(e, message, returnedData);
                }
            }
        }

        public void AfterMessageExecution(BasicDeliverEventArgs e, Message message, ExecResult returnedData)
        {
            if (returnedData.ErrorCode == 0)
            {
                helper.Logger.Log($"Done executing message id ({message.MessageID})");
                MessageAknowledge(e, RabbitMQMessageState.SuccessfullyProcessed);
            }
            else
            {
                dbHelper.RecordMessageFailure(message, $"Failed to execute, {returnedData.ErrorException}");
                MessageAknowledge(e, RabbitMQMessageState.UnsuccessfulProcessing);
            }
        }

        public void MessageAknowledge(BasicDeliverEventArgs e, RabbitMQMessageState state)
        {
            switch (state)
            {
                case RabbitMQMessageState.SuccessfullyProcessed:
                    // Success remove from queue
                    helper.Logger.Log("Success remove from queue");
                    RabbitMQChannel.BasicAck(e.DeliveryTag,false);
                    break;
                case RabbitMQMessageState.UnsuccessfulProcessing:
                    // Unsuccessful, requeue and retry
                    helper.Logger.Log("Unsuccessful, requeue and retry");
                    RabbitMQChannel.BasicNack(e.DeliveryTag, false, true);
                    break;
                default:
                    // Bad Message, Reject and Delete
                    helper.Logger.Log("Bad Message, Reject and Delete");
                    RabbitMQChannel.BasicReject(e.DeliveryTag, false);
                    break;
            }
        }
    }
}
