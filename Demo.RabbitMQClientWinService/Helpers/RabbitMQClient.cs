using Buddy.Utilities;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQClientWinService.Helpers
{
    public class RabbitMQClient : IRabbitMQClient
    {
        DBHelper dbHelper;
        Helper helper;
        public RabbitMQClient()
        {
            dbHelper = Helper.CreateInstance<DBHelper>();
            helper = Helper.CreateInstance();
        }
        public IModel RabbitMQChannel { get; set; }
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
                helper.Log($"Rabbit MQ Exception: {ex.ToString()}");
            }
        }

        public void PublishNewMessages(List<Message> messages)
        {
            try
            {
                string demoQueue = "demo-queue";
                this.EstablishRabbitMQ();
                RabbitMQChannel.QueueDeclare(demoQueue, durable: true, exclusive: false, autoDelete: false, arguments: null);
                helper.Log($"/////////////////////// Publishing {messages.Count} messages /////////////////////////////");
                foreach (Message msg in messages)
                {
                    PublishMessage(demoQueue, msg);
                }
                helper.Log($"Done publishing messages...");
            }
            catch (Exception ex)
            {
                helper.Log($"Exception: {ex.ToString()}");
            }
        }

        public void PublishMessage(string queue, Message msg)
        {
            try
            {
                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(msg));
                string exchangeType = ""; // "" = default exchange
                var msgProps = RabbitMQChannel.CreateBasicProperties();
                msgProps.DeliveryMode = 2; // presistent message
                msgProps.Persistent = true; 
                msgProps.Expiration = "36000000"; // in millisecond
                RabbitMQChannel.BasicPublish(exchangeType, queue, msgProps, body);
            }
            catch (Exception ex)
            {
                dbHelper.RecordMessageFailure(msg, $"Failed to publish: {ex.ToString()}");
            }
        }

        public void ConsumeNewMessages()
        {
            helper.Log("Consumer started..");
            string demoQueue = "demo-queue";
            this.EstablishRabbitMQ();
            RabbitMQChannel.QueueDeclare(demoQueue, durable: true, exclusive: false, autoDelete: false, arguments: null);
            var consumer = new EventingBasicConsumer(RabbitMQChannel);
            consumer.Received += (sender, e) =>
            {
                ConsumeMessageAsync(e);
            };
            RabbitMQChannel.BasicConsume(demoQueue, false, consumer);
        }

        public void ConsumeMessageAsync(BasicDeliverEventArgs e)
        {
            helper.Log($"////////////////////////// Message Received ///////////////////////////");
            var body = e.Body.ToArray();
            var messageJson = Encoding.UTF8.GetString(body);
            Message message = JsonConvert.DeserializeObject<Message>(messageJson);
            if (string.IsNullOrEmpty(message.MessageData) || message.MessageData == "INVALID")
            {
                helper.Log($"Invalid Message: message id ({message.MessageID}) has invalid data!");
                MessageAknowledge(e, ReceivedMessageState.MessageRejected);
            }
            else
            {
                Task.Factory.StartNew(() => { return dbHelper.ExecuteMQMessage(message); }).ContinueWith((taskExec) =>
                {
                    if (taskExec.Result > 0)
                    {
                        helper.Log($"Done executnig message id ({message.MessageID})");
                        MessageAknowledge(e, ReceivedMessageState.SuccessfullyProcessed);
                    }
                    else
                    {
                        dbHelper.RecordMessageFailure(message, "Failed to execute, please review the logs");
                        MessageAknowledge(e, ReceivedMessageState.UnsuccessfulProcessing);
                    }
                });
            }
        }

        public void MessageAknowledge(BasicDeliverEventArgs e, ReceivedMessageState state)
        {
            switch (state)
            {
                case ReceivedMessageState.SuccessfullyProcessed:
                    // Success remove from queue
                    RabbitMQChannel.BasicAck(deliveryTag: e.DeliveryTag, multiple: false);
                    break;
                case ReceivedMessageState.UnsuccessfulProcessing:
                    // Unsuccessful, requeue and retry
                    RabbitMQChannel.BasicNack(deliveryTag: e.DeliveryTag, multiple: false, requeue: true);
                    break;
                default:
                    // Bad Message, Reject and Delete
                    RabbitMQChannel.BasicReject(deliveryTag: e.DeliveryTag, requeue: false);
                    break;
            }
        }
    }

    public enum ReceivedMessageState : int
    {
        Unknown = 0,
        SuccessfullyProcessed = 1,
        UnsuccessfulProcessing = 2,
        MessageRejected = 3
    }
}
