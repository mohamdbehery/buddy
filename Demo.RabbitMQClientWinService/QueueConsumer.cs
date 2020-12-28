using Buddy.Utilities;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQClientWinService.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQClientWinService
{
    public static class QueueConsumer
    {
        static Helper helper = new Helper();
        static DBHelper dbHelper = new DBHelper();
        public static void ConsumeNewMessages(IModel channel)
        {
            helper.Log("Consumer started..");
            string demoQueue = "demo-queue";
            channel.QueueDeclare(demoQueue, durable: true, exclusive: false, autoDelete: false, arguments: null);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, e) =>
            {
                helper.Log($"////////////////////////// Message Received ///////////////////////////");
                var body = e.Body.ToArray();
                var messageJson = Encoding.UTF8.GetString(body);
                Message message = JsonConvert.DeserializeObject<Message>(messageJson);
                if (dbHelper.ExecuteMQMessage(message) > 0)
                    helper.Log($"Done executnig message: {message.MessageID}");
                else
                {
                    dbHelper.RecordMessageFailure(message, "Failed to execute, please review the logs");
                }
            };
            channel.BasicConsume(demoQueue, true, consumer);
        }
    }
}
