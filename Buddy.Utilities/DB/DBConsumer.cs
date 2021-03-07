using Buddy.Utilities.Enums;
using Buddy.Utilities.Models;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using static Buddy.Utilities.Enums.HelperEnums;

namespace Buddy.Utilities.DB
{
    public class DBConsumer: HelperBase
    {
        private SqlConnection SQLConnection;
        private SqlCommand SQLCommand;
        private SqlDataAdapter SQLDataAdapter;
        private string[] SQLBinaryParams;
        private ExecResult execResult;
        private Logger logger;

        public DBConsumer()
        {
            SQLBinaryParams = new string[] { "@HRPersonalPhoto", "@FollowUpAttachmentFilePath" };
            logger = new Logger();
        }

        public ExecResult CallSQLDB(DBExecParams dBExecParams)
        {
            bool isStoredProcedure = string.IsNullOrEmpty(dBExecParams.StoredProcedure) ? false : true;
            bool isSQLFile = string.IsNullOrEmpty(dBExecParams.SQLFilePath) ? false : true;
            if (isSQLFile)
                dBExecParams.Query = ExtractQueryFromSQLFile(dBExecParams);

            execResult = new ExecResult();
            execResult.ResultSet = new DataSet();
            try
            {
                using (SQLConnection = new SqlConnection(dBExecParams.ConString))
                {
                    SQLConnection.InfoMessage += new SqlInfoMessageEventHandler((object sender, SqlInfoMessageEventArgs e) =>
                    {
                        SQLMessageHandler(sender, e, ref execResult);
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
                                SQLDataAdapter = new SqlDataAdapter(SQLCommand);
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
                foreach (var param in dBExecParams.Parameters)
                {
                    paramsValues += " SET " + param.Key + "=" + (!string.IsNullOrEmpty(param.Value) ? (param.Key.Contains("CODE") ? "'" + param.Value + "'" : param.Value) : "NULL; ");
                }
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

        private void SQLMessageHandler(object sender, SqlInfoMessageEventArgs e, ref ExecResult returnedData)
        {
            // This gets all the messages generated during the execution of the SQL, 
            // including low-severity error messages.
            foreach (SqlError err in e.Errors)
            {
                returnedData.ExecutionMessages += $"  // $$ // {err.Procedure} line: {err.LineNumber} >> {err.Message}";
            }
        }
    }   
}
