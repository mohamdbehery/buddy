using Buddy.Utilities;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQClientWinService.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQClientWinService
{
    public static class QueuePublisher
    {
        static Helper helper = new Helper();
        static DBHelper dbHelper = new DBHelper();
        public static void PublishNewMessages(IModel channel)
        {
            try
            {
                string demoQueue = "demo-queue";
                channel.QueueDeclare(demoQueue, durable: true, exclusive: false, autoDelete: false, arguments: null);
                List<Message> messages = dbHelper.FetchMQMessages();
                if (messages.Count > 0)
                {
                    helper.Log($"/////////////////////// Publishing {messages.Count} messages /////////////////////////////");
                    foreach (Message msg in messages)
                    {
                        PublishMessage(channel, demoQueue, msg);
                    }
                    helper.Log($"Done publishing messages...");
                }
            }
            catch (Exception ex)
            {
                helper.Log($"Exception: {ex.ToString()}");
            }
        }

        public static void PublishMessage(IModel channel, string queue, Message msg)
        {
            try
            {
                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(msg));
                channel.BasicPublish("", queue, null, body);
            }
            catch (Exception ex)
            {
                dbHelper.RecordMessageFailure(msg, $"Failed to publish: {ex.ToString()}");
            }
        }
    }
}
