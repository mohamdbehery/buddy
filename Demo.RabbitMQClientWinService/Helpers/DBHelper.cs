using Buddy.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQClientWinService.Helpers
{
    public class DBHelper
    {
        static Helper helper = Helper.CreateInstance();
        string conStr = "";
        public DBHelper()
        {
            conStr = helper.GetAppKey("conStr");
        }

        public List<Message> FetchMQMessages()
        {
            List<Message> messages = new List<Message>();
            DBExecResult execResult = helper.DBExecution(new DBExecParams() { ConString = conStr, StoredProcedure = "spFetchMessages", ExecType = DBExecType.DataAdapter });
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

        public DBExecResult ExecuteMQMessage(Message message)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>()
            {
                {"@MessageID", message.MessageID.ToString()},
            };
            DBExecResult execResult = helper.DBExecution(new DBExecParams() { ConString = conStr, StoredProcedure = "spStartMessageExecution", Parameters = parameters, ExecType = DBExecType.ExecuteNonQuery });
            if (execResult.ErrorCode == 0)
            {
                helper.Log($"Start executing message: {message.MessageID}");
                int timeToSleep = 10000;
                if(message.MessageData.Split('$').Count() == 2)
                {
                    var tempMsg = message.MessageData.Split('$');
                    timeToSleep = int.Parse(message.MessageData.Split('$')[1]) * 1000;
                }
                Thread.Sleep(timeToSleep);
                parameters.Add("@MessageData", message.MessageData.ToString());
                execResult = helper.DBExecution(new DBExecParams() { ConString = conStr, StoredProcedure = "spFinishMessageExecution", Parameters = parameters, ExecType = DBExecType.ExecuteNonQuery });
            }

            return execResult;
        }

        public void RecordMessageFailure(Message msg, string failureMessage)
        {
            helper.Log(failureMessage);
            Dictionary<string, string> parameters = new Dictionary<string, string>()
                {
                    {"@MessageID", msg.MessageID.ToString()},
                    {"@FailureMessage", failureMessage}
                };
            DBExecResult returnedData = helper.DBExecution(new DBExecParams() { ConString = conStr, StoredProcedure = "spUpdateMessageFailure", Parameters = parameters, ExecType = DBExecType.ExecuteNonQuery });
        }
    }
}
