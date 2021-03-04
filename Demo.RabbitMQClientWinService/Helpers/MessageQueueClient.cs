using Buddy.Utilities;
using Buddy.Utilities.Models;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Buddy.Utilities.HelperEnums;
using static RabbitMQClientWinService.Helpers.Enums;

namespace RabbitMQClientWinService.Helpers
{
    public abstract class MessageQueueClient
    {
        public Helper helper;
        public string conStr = "";
        public MessageQueueClient()
        {
            helper = Helper.CreateInstance();
            conStr = helper.GetAppKey("conStr");
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
        public List<Message> FetchMQMessages()
        {
            List<Message> messages = new List<Message>();
            ExecResult execResult = helper.DBConsumer.CallSQLDB(new DBExecParams() { 
                ConString = conStr, StoredProcedure = "spFetchMessages", ExecType = DBExecType.DataAdapter, Parameters = new Dictionary<string, string>() { { "@FetchMessageCount", MessageCountToFetch.ToString() } }
            });
            if (execResult.ErrorCode == 0)
            {
                if (execResult.ResultSet.Tables.Count > 0)
                {
                    foreach (DataRow item in execResult.ResultSet.Tables[0].Rows)
                    {
                        messages.Add(new Message()
                        {
                            MessageID = Convert.ToInt32(item["Id"]),
                            MessageData = Convert.ToString(item["MessageData"])
                        });
                    }
                }
            }
            return messages;
        }
        public abstract void PublishNewMessages(List<Message> messages);
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
        public ExecResult ExecuteMQMessage(Message message)
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
        public void AfterMessageExecution(BasicDeliverEventArgs e, Message message, ExecResult returnedData)
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
        public void RecordMessageFailure(Message msg, string failureMessage)
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
