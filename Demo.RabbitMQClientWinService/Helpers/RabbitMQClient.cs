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
        public IModel Channel { get; set; }
        public void EstablishRabbitMQ()
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    Uri = new Uri(helper.GetAppKey("RabbitMQUri"))
                };
                var connection = factory.CreateConnection();
                Channel = connection.CreateModel();
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
                Channel.QueueDeclare(demoQueue, durable: true, exclusive: false, autoDelete: false, arguments: null);
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
                Channel.BasicPublish("", queue, null, body);
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
            Channel.QueueDeclare(demoQueue, durable: true, exclusive: false, autoDelete: false, arguments: null);
            var consumer = new EventingBasicConsumer(Channel);
            consumer.Received += (sender, e) =>
            {
                ConsumeMessageAsync(e);
            };
            Channel.BasicConsume(demoQueue, true, consumer);
        }

        public async void ConsumeMessageAsync(BasicDeliverEventArgs e)
        {
            helper.Log($"////////////////////////// Message Received ///////////////////////////");
            var body = e.Body.ToArray();
            var messageJson = Encoding.UTF8.GetString(body);
            Message message = JsonConvert.DeserializeObject<Message>(messageJson);
            var affectedRows = await Task.Factory.StartNew(() => { return dbHelper.ExecuteMQMessage(message); });
            if (affectedRows > 0)
                helper.Log($"Done executnig message: {message.MessageID}");
            else
            {
                dbHelper.RecordMessageFailure(message, "Failed to execute, please review the logs");
            }
        }
    }
}
