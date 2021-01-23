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
            DataTable dtMessages = new DataTable();
            ReturnedData returnedData = helper.GetDTFromSQLDB_SP(conStr, "spFetchMessages", null, out dtMessages);
            if (returnedData.errorCode == 0)
            {
                foreach (DataRow item in dtMessages.Rows)
                {
                    messages.Add(new Message()
                    {
                        MessageID = Convert.ToInt32(item["Id"]),
                        MessageData = Convert.ToString(item["MessageData"])
                    });
                }
            }
            else
                helper.Log($"DB Error (spFetchMessages) {returnedData.errorException}");
            
            return messages;
        }

        public ReturnedData ExecuteMQMessage(Message message)
        {
            Dictionary<string, string> Params = new Dictionary<string, string>()
            {
                {"@MessageID", message.MessageID.ToString()},
            };
            int affectedRows = 0;
            ReturnedData returnedData = helper.ExecuteSQLDB_SP(conStr, "spStartMessageExecution", Params, out affectedRows);
            if (returnedData.errorCode == 0)
            {
                helper.Log($"Start executing message: {message.MessageID}");
                int timeToSleep = 10000;
                if(message.MessageData.Split('$').Count() == 2)
                {
                    var tempMsg = message.MessageData.Split('$');
                    timeToSleep = int.Parse(message.MessageData.Split('$')[1]) * 1000;
                }
                Thread.Sleep(timeToSleep);
                Params.Add("@MessageData", message.MessageData.ToString());
                affectedRows = 0;
                returnedData = helper.ExecuteSQLDB_SP(conStr, "spFinishMessageExecution", Params, out affectedRows);
                if (returnedData.errorCode != 0)
                    helper.Log($"DB Error (spFinishMessageExecution) {returnedData.errorException}");
            }
            else
                helper.Log($"DB Error (spStartMessageExecution) {returnedData.errorException}");

            return returnedData;
        }

        public void RecordMessageFailure(Message msg, string failureMessage)
        {
            helper.Log(failureMessage);
            Dictionary<string, string> Params = new Dictionary<string, string>()
                {
                    {"@MessageID", msg.MessageID.ToString()},
                    {"@FailureMessage", failureMessage}
                };
            int affectedRows = 0;
            ReturnedData returnedData = helper.ExecuteSQLDB_SP(conStr, "spUpdateMessageFailure", Params, out affectedRows);
            if (returnedData.errorCode != 0)
                helper.Log($"DB Error (spUpdateMessageFailure) {returnedData.errorException}");
        }
    }
}
