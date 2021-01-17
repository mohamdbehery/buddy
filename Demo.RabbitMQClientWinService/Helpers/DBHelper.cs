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
            try
            {
                DataTable dtMessages = new DataTable();
                using (SqlConnection SQLCon = new SqlConnection(conStr))
                {
                    using (SqlCommand SQLCMD = new SqlCommand("spFetchMessages", SQLCon))
                    {
                        SQLCMD.CommandType = CommandType.StoredProcedure;
                        SQLCon.Open();
                        SqlDataAdapter SQLDataAd = new SqlDataAdapter(SQLCMD);
                        SQLDataAd.Fill(dtMessages);

                        SQLCon.Close();
                        SQLDataAd.Dispose();
                    }
                }
                foreach (DataRow item in dtMessages.Rows)
                {
                    messages.Add(new Message()
                    {
                        MessageID = Convert.ToInt32(item["Id"]),
                        MessageData = Convert.ToString(item["MessageData"])
                    });
                }
            }
            catch (Exception ex)
            {
                helper.Log($"Exception: {ex.ToString()}");
            }
            return messages;
        }

        public int UpdateMQMeesage(string procedure, Dictionary<string, string> Params)
        {
            int rows = 0;
            using (SqlConnection SQLCon = new SqlConnection(conStr))
            {
                using (SqlCommand SQLCMD = new SqlCommand(procedure, SQLCon))
                {
                    SQLCMD.CommandType = CommandType.StoredProcedure;
                    if (Params != null && Params.Count > 0)
                    {
                        foreach (var Param in Params)
                        {
                            SQLCMD.Parameters.Add(new SqlParameter(Param.Key, Param.Value));
                        }
                    }
                    SQLCon.Open();
                    rows = SQLCMD.ExecuteNonQuery();
                    SQLCon.Close();
                }
            }
            return rows;
        }

        public int ExecuteMQMessage(Message message)
        {
            Dictionary<string, string> Params = new Dictionary<string, string>()
            {
                {"@MessageID", message.MessageID.ToString()},
            };
            if (UpdateMQMeesage("spStartMessageExecution", Params) > 0)
            {
                helper.Log($"Start executing message: {message.MessageID}");
                Thread.Sleep(20000);
                Params.Add("@MessageData", message.MessageData.ToString());
                return UpdateMQMeesage("spFinishMessageExecution", Params);
            }
            return 0;
        }

        public void RecordMessageFailure(Message msg, string failureMessage)
        {
            helper.Log(failureMessage);
            Dictionary<string, string> Params = new Dictionary<string, string>()
                {
                    {"@MessageID", msg.MessageID.ToString()},
                    {"@FailureMessage", failureMessage}
                };
            UpdateMQMeesage("spUpdateMessageFailure", Params);
        }
    }
}
