using Buddy.Utilities.Enums;
using Buddy.Utilities.Models;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using static Buddy.Utilities.Enums.HelperEnums;

namespace Buddy.Utilities.DB
{
    // sealed here to prevent inheritance
    public sealed class DBConsumer : HelperBase
    {
        private SqlConnection SQLConnection;
        private SqlCommand SQLCommand;
        private ExecResult execResult;
        private readonly Logger logger;
        private readonly string[] SQLBinaryParams;

        public DBConsumer()
        {
            logger = new Logger();
            SQLBinaryParams = new string[] { "@HRPersonalPhoto", "@FollowUpAttachmentFilePath" };
        }

        public ExecResult CallSQLDB(DBExecParams dBExecParams)
        {
            execResult = new ExecResult();
            execResult.ResultSet = new DataSet();
            bool isStoredProcedure = !string.IsNullOrEmpty(dBExecParams.StoredProcedure);
            bool isSQLFile = !string.IsNullOrEmpty(dBExecParams.SQLFilePath);
            if (isSQLFile)
                dBExecParams.Query = ExtractQueryFromSQLFile(dBExecParams);

            try
            {
                using (SQLConnection = new SqlConnection(dBExecParams.ConString))
                {
                    SQLConnection.InfoMessage += new SqlInfoMessageEventHandler((object sender, SqlInfoMessageEventArgs e) =>
                    {
                        SQLMessageHandler(e, ref execResult);
                    });

                    using (SQLCommand = new SqlCommand(isStoredProcedure ? dBExecParams.StoredProcedure : dBExecParams.Query, SQLConnection))
                    {
                        SQLCommand.CommandType = isStoredProcedure ? CommandType.StoredProcedure : CommandType.Text;
                        if (!isSQLFile && dBExecParams.Parameters != null && dBExecParams.Parameters.Count > 0)
                        {
                            FillSQLParams(dBExecParams, ref SQLCommand);
                        }
                        SQLConnection.Open();
                        switch (dBExecParams.ExecType)
                        {
                            case DBExecType.ExecuteNonQuery:
                                execResult.AffectedRowsCount = SQLCommand.ExecuteNonQuery();
                                break;
                            case DBExecType.DataAdapter:
                                SqlDataAdapter SQLDataAdapter = new SqlDataAdapter(SQLCommand);
                                SQLDataAdapter.Fill(execResult.ResultSet);
                                SQLDataAdapter.Dispose();
                                break;
                            case DBExecType.ExecuteScalar:
                                execResult.ResultField = SQLCommand.ExecuteScalar().ToString();
                                break;
                        }
                        SQLConnection.Close();
                    }
                }
                execResult.ErrorException = null;
                return execResult;
            }
            catch (Exception ex)
            {
                execResult.AffectedRowsCount = 0;
                execResult.ErrorCode = HelperEnums.ErrorCode.Exception;
                execResult.ErrorException = $"{Constants.ADONETException}: {ex.ToString()}";
                logger.Log($"//_-_\\ {dBExecParams.StoredProcedure} {dBExecParams.Query}: {execResult.ErrorException}");
                return execResult;
            }
        }

        private string ExtractQueryFromSQLFile(DBExecParams dBExecParams)
        {
            string Query = File.ReadAllText(dBExecParams.SQLFilePath).Replace("\n", " \n ");
            if (dBExecParams.WordsToKeepInSQLFile != null)
            {
                foreach (var word in dBExecParams.WordsToKeepInSQLFile)
                {
                    Query = Query.Replace(word, SpreadWord(word));
                }
            }
            if (dBExecParams.WordsToDeleteFromSQLFile != null)
            {
                foreach (var word in dBExecParams.WordsToDeleteFromSQLFile)
                {
                    Query = Query.Replace(word, " ");
                    Query = Query.Replace(" " + word + " ", " ");
                    Query = Query.Replace(" " + word, " ");
                    Query = Query.Replace(word + " ", " ");
                }
            }

            if (dBExecParams.WordsToKeepInSQLFile != null)
            {
                foreach (var word in dBExecParams.WordsToKeepInSQLFile)
                {
                    Query = Query.Replace(SpreadWord(word), word);
                }
            }
            string paramsValues = "";
            if (dBExecParams.Parameters != null && dBExecParams.Parameters.Count > 0)
            {
                StringBuilder sqlScripts = new StringBuilder();
                foreach (var param in dBExecParams.Parameters)
                {
                    sqlScripts.Append(" SET " + param.Key + "=" + (!string.IsNullOrEmpty(param.Value) ? (param.Key.Contains("CODE") ? $"'{param.Value}'" : param.Value) : "NULL; "));
                    
                }
                paramsValues = sqlScripts.ToString();
            }

            if (!string.IsNullOrEmpty(dBExecParams.KeywordToSetParamsValue))
                Query = Query.Replace(dBExecParams.KeywordToSetParamsValue, paramsValues);

            return Query;
        }
        private void FillSQLParams(DBExecParams dBExecParams, ref SqlCommand sqlCommand)
        {
            foreach (var param in dBExecParams.Parameters)
            {
                if (string.IsNullOrEmpty(param.Value))
                    sqlCommand.Parameters.Add(new SqlParameter(param.Key, DBNull.Value));
                else
                {
                    if (SQLBinaryParams.Contains(param.Key))
                    {
                        byte[] ParamValue = ConvertFileBase64StringToByteArray(param.Value);
                        sqlCommand.Parameters.Add(new SqlParameter(param.Key, SqlDbType.VarBinary, ParamValue.Length)).Value = ParamValue;
                    }
                    else
                        sqlCommand.Parameters.Add(new SqlParameter(param.Key, param.Value));
                }
            }
        }

        private void SQLMessageHandler(SqlInfoMessageEventArgs e, ref ExecResult returnedData)
        {
            // This gets all the messages generated during the execution of the SQL, 
            // including low-severity error messages.
            StringBuilder messages = new StringBuilder();
            foreach (SqlError err in e.Errors)
            {
                messages.Append($" // $$ // {err.Procedure} line: {err.LineNumber} >> {err.Message}");
                
            }
            returnedData.ExecutionMessages += messages.ToString();
        }
    }
}
