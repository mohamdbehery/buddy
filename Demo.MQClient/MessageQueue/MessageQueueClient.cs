using Buddy.Utilities;
using Buddy.Utilities.Enums;
using Buddy.Utilities.Models;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using RabbitMQClientWinService.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Buddy.Utilities.Enums.HelperEnums;
using static RabbitMQClientWinService.Models.MQEnums;

namespace RabbitMQClientWinService.MessageQueue
{
    public abstract class MessageQueueClient
    {
        // represents a delegate
        public event EventHandler MessengerStarted;

        public Helper helper = new Helper();
        private string conStr
        {
            get
            {
                return helper.GetAppKey("conStr");
            }
        }
        public MessageQueueClient()
        {
            
        }

        protected void OnMessengerStarted()
        {
            if (MessengerStarted != null)
                MessengerStarted(this, EventArgs.Empty);
        }
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

        public int FetchMessagesTimeIntervalInMSs
        {
            get
            {
                int interval = 0;
                return int.TryParse(helper.GetAppKey("FetchMessagesTimeIntervalInMSs"), out interval) ? interval : 5000;
            }
        }

        public abstract int MessageCountToFetch { get; }
        public abstract void StartMessenger();
        public List<MQMessage> FetchMQMessages()
        {
            List<MQMessage> messages = new List<MQMessage>();
            ExecResult execResult = helper.DBConsumer.CallSQLDB(new DBExecParams() { 
                ConString = conStr, StoredProcedure = "spFetchMessages", ExecType = DBExecType.DataAdapter, Parameters = new Dictionary<string, string>() { { "@FetchMessageCount", MessageCountToFetch.ToString() } }
            });
            if (execResult.ErrorCode == 0)
            {
                if (execResult.ResultSet.Tables.Count > 0)
                {
                    foreach (DataRow item in execResult.ResultSet.Tables[0].Rows)
                    {
                        messages.Add(new MQMessage()
                        {
                            MessageID = Convert.ToInt32(item["Id"]),
                            MessageData = Convert.ToString(item["MessageData"])
                        });
                    }
                }
            }
            helper.Logger.Log($"{messages.Count()} messages fetched...");
            return messages;
        }
        public abstract void PublishNewMessages(List<MQMessage> messages);
        public void ConsumeMessage(BasicDeliverEventArgs e, MQMessage message = null)
        {
            helper.Logger.Log($"///////////// Message Received /////////////");
            if (message == null)
            {
                var body = e.Body.ToArray();
                var messageJson = Encoding.UTF8.GetString(body);
                message = JsonConvert.DeserializeObject<MQMessage>(messageJson);
            }
            if (string.IsNullOrEmpty(message.MessageData) || message.MessageData == "INVALID")
            {
                RecordMessageFailure(message, $"Invalid Message: message id ({message.MessageID}) has invalid data!");
                MessageAknowledge(MQMessageState.MessageRejected, e);
            }
            else
            {
                if (ParallelExecuteMessages)
                {
                    Task.Factory.StartNew(() => { return ExecuteMQMessage(message); }).ContinueWith((taskExec) =>
                    {
                        AfterMessageExecution(e, message, taskExec.Result);
                    });
                }
                else
                {
                    ExecResult execResult = ExecuteMQMessage(message);
                    AfterMessageExecution(e, message, execResult);
                }
            }
        }
        public ExecResult ExecuteMQMessage(MQMessage message)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>()
            {
                {"@MessageID", message.MessageID.ToString()},
            };
            ExecResult execResult = helper.DBConsumer.CallSQLDB(new DBExecParams() { ConString = conStr, StoredProcedure = "spStartMessageExecution", Parameters = parameters, ExecType = DBExecType.ExecuteNonQuery });
            if (execResult.ErrorCode == 0)
            {
                helper.Logger.Log($"Start executing message: {message.MessageID}");
                int timeToSleep = 10000;
                if (message.MessageData.Split('$').Count() == 2)
                {
                    var tempMsg = message.MessageData.Split('$');
                    timeToSleep = int.Parse(message.MessageData.Split('$')[1]) * 1000;
                }
                Thread.Sleep(timeToSleep);
                parameters.Add("@MessageData", message.MessageData.ToString());
                execResult = helper.DBConsumer.CallSQLDB(new DBExecParams() { ConString = conStr, StoredProcedure = "spFinishMessageExecution", Parameters = parameters, ExecType = DBExecType.ExecuteNonQuery });
            }

            return execResult;
        }
        public void AfterMessageExecution(BasicDeliverEventArgs e, MQMessage message, ExecResult returnedData)
        {
            if (returnedData.ErrorCode == 0)
            {
                helper.Logger.Log($"Done executing message id ({message.MessageID})");
                MessageAknowledge(MQMessageState.SuccessfullyProcessed, e);
            }
            else
            {
                RecordMessageFailure(message, $"Failed to execute, {returnedData.ErrorException}");
                MessageAknowledge(MQMessageState.UnsuccessfulProcessing, e);
            }
        }
        public abstract void MessageAknowledge(MQMessageState state, BasicDeliverEventArgs e = null);
        public void RecordMessageFailure(MQMessage msg, string failureMessage)
        {
            helper.Logger.Log(failureMessage);
            Dictionary<string, string> parameters = new Dictionary<string, string>()
                {
                    {"@MessageID", msg.MessageID.ToString()},
                    {"@FailureMessage", failureMessage}
                };
            ExecResult execResult = helper.DBConsumer.CallSQLDB(new DBExecParams() { ConString = conStr, StoredProcedure = "spUpdateMessageFailure", Parameters = parameters, ExecType = DBExecType.ExecuteNonQuery });
            if (execResult.ErrorCode != HelperEnums.ErrorCode.Zero)
                helper.Logger.Log("failed to log exception into Database!!");
        }
    }
}
