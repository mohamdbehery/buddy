using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Web.Administration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace Buddy.Utilities
{
    public class Helper
    {
        public int errorCode = 100;
        CommonProps _props = new CommonProps();
        private XmlDocument xmlDoc;
        private XmlNode rootNode;
        private XmlNode node;
        private XmlAttribute xmlAttribute;
        private DataTable dt1;
        private DataSet ds1;
        private SqlConnection sqlConnection;
        private SqlCommand sqlCommand;
        private SqlDataAdapter sqlDataAdapter;
        FileStream logsFileStream;
        StreamWriter logsStreamWriter;
        private string[] CustomParams = { "@HRPersonalPhoto", "@FollowUpAttachmentFilePath" };

        public Helper(bool initLogging)
        {

        }

        private Helper()
        {

        }

        public string LogFilePath
        {
            get
            {
                string logFileExtension = ".txt";
                string LogFileNamePrefix = "Log";
                string projectName = "UnknownSource";
                int fileMaxSizeInBytes = 2000000;
                string LogsDirectory = @"C:\Inetpub\BuddyLogger";
                string currentDate = DateTime.Now.ToString("MM-dd-yyyy");
                string currentHour = DateTime.Now.Hour.ToString();

                if (!string.IsNullOrEmpty(GetAppKey("LogsMaxFileSize")))
                {
                    int temp = 0;
                    if (int.TryParse(GetAppKey("LogsMaxFileSize"), out temp))
                        fileMaxSizeInBytes = temp;
                }

                if (!string.IsNullOrEmpty(GetAppKey("LogsDicrectory")))
                    LogsDirectory = GetAppKey("LogsDicrectory");

                StackTrace stackTrace = new StackTrace();
                try
                {
                    string assemblyName = Assembly.GetEntryAssembly().ManifestModule.Name;
                    projectName = assemblyName.Remove(assemblyName.IndexOf('.'));
                }
                catch { }

                if (!Directory.Exists(LogsDirectory))
                    Directory.CreateDirectory(LogsDirectory);

                string DayLogsDirectory = Path.Combine(LogsDirectory, currentDate);
                if (!Directory.Exists(DayLogsDirectory))
                    Directory.CreateDirectory(DayLogsDirectory);

                string ProjectDirectory = Path.Combine(DayLogsDirectory, projectName);
                if (!Directory.Exists(ProjectDirectory))
                    Directory.CreateDirectory(ProjectDirectory);

                string hourLogsFilePath = Path.Combine(ProjectDirectory, $"{LogFileNamePrefix}-{currentDate}-{currentHour}-1{logFileExtension}");

                if (File.Exists(hourLogsFilePath))
                {
                    FileInfo logFileInfo = new FileInfo(hourLogsFilePath);
                    if (logFileInfo.Length > fileMaxSizeInBytes)
                    {
                        string logFileName = Path.GetFileNameWithoutExtension(hourLogsFilePath);
                        string[] logFileNameParts = logFileName.Split('-');
                        int fileVersion = 0;
                        if (int.TryParse(logFileNameParts[logFileNameParts.Length - 1], out fileVersion))
                        {
                            fileVersion++;
                            logFileNameParts[logFileNameParts.Length - 1] = fileVersion.ToString();
                            hourLogsFilePath = Path.Combine(ProjectDirectory, $"{string.Join("-", logFileNameParts)}{logFileExtension}");
                        }
                    }
                }
                return hourLogsFilePath;
            }
        }

        public static Helper CreateInstance()
        {
            return new Helper();
        }
        public static Type CreateInstance<Type>()
        {
            Type objectInstance = (Type)Activator.CreateInstance(typeof(Type));
            return objectInstance;
        }

        public string GetConfigKey(string Key)
        {
            return ConfigurationManager.ConnectionStrings[Key].ToString();
        }

        public string GetAppKey(string Key)
        {
            if (ConfigurationManager.AppSettings[Key] != null)
                return ConfigurationManager.AppSettings[Key].ToString();
            return string.Empty;
        }

        public string GetSubString(string content, string from, string to)
        {
            int FromIndex = content.IndexOf(from) + from.Length;
            int ToIndex = content.LastIndexOf(to);
            return content.Substring(FromIndex, ToIndex - FromIndex);
        }

        public string RemoveSubString(string content, string from, string to)
        {
            if (content.Contains(from) && content.Contains(to))
            {
                int FromIndex = content.IndexOf(from);
                int ToIndex = content.IndexOf(to, FromIndex);
                return content.Remove(FromIndex, ((ToIndex + to.Length) - FromIndex));
            }
            else
                return content;
        }

        public string BindStringAttributes(string content, XmlNode source)
        {
            foreach (XmlAttribute Item in source.Attributes)
            {
                if (content.Contains("#" + Item.Name.ToString() + "#"))
                    content = content.Replace("#" + Item.Name.ToString() + "#", Item.Value);
            }
            return content;
        }

        public string ReplaceDelimeterInString(string Content, string Delimeter, string Replacement)
        {
            int Counter = 10;
            for (int i = 0; i < Counter; i++)
            {
                if (Content.Contains(Delimeter))
                    Content = Content.Replace(Delimeter, Replacement);
            }
            return Content;
        }

        public string GenerateRandomNumber()
        {
            Random Random = new Random();
            int randomNumber = Random.Next(0, 1000);
            return randomNumber.ToString("000") + DateTime.Now.Millisecond;
        }

        public void CalcQuotientRemainder(Decimal Numerator, Decimal Denominator, out int Quotient, out Decimal Remainder)
        {
            Quotient = Convert.ToInt32(Math.Floor(Numerator / Denominator));
            Remainder = Numerator % Denominator;
        }

        public string SerializeDataSetToXml(DataSet dsTemp)
        {
            if (dsTemp == null) return string.Empty;
            using (var strm = new StringWriter())
            {
                if (string.IsNullOrEmpty(dsTemp.DataSetName))
                    dsTemp.DataSetName = string.Format("TempDataSet{0}", new Random().Next(0, 100));
                dsTemp.WriteXml(strm);
                return strm.ToString();
            }
        }

        public string SerializeDataTableToXml(DataTable dtTemp)
        {
            if (dtTemp == null) return string.Empty;
            using (var strm = new StringWriter())
            {
                if (string.IsNullOrEmpty(dtTemp.TableName))
                    dtTemp.TableName = string.Format("TempTable{0}", new Random().Next(0, 100));
                dtTemp.WriteXml(strm);
                return strm.ToString();
            }
        }

        public string SerializeDictionaryToString(Dictionary<string, string> Dict)
        {
            return string.Join(";", Dict.Select(x => x.Key + "=" + x.Value).ToArray());
        }

        public Dictionary<string, string> GetAllAppKeys()
        {
            Dictionary<string, string> dicAllKeys = new Dictionary<string, string>();
            XmlDocument document = new XmlDocument();
            document.Load(HttpContext.Current.Request.PhysicalApplicationPath + "/Web.config");
            XmlNodeList AllKeys = document.SelectNodes("/configuration/appSettings/add");

            foreach (XmlNode node in AllKeys)
            {
                dicAllKeys.Add(node.Attributes["key"].Value, node.Attributes["value"].Value);
            }
            return dicAllKeys;  
        }

        public DataTable ReadExceltoDT(string ExcelFilePath)
        {
            DataTable DT = new DataTable();
            using (SpreadsheetDocument doc = SpreadsheetDocument.Open(ExcelFilePath, false))
            {
                Sheet sheet = doc.WorkbookPart.Workbook.Sheets.GetFirstChild<Sheet>();
                Worksheet worksheet = (doc.WorkbookPart.GetPartById(sheet.Id.Value) as WorksheetPart).Worksheet;
                IEnumerable<Row> rows = worksheet.GetFirstChild<SheetData>().Descendants<Row>();
                foreach (Row row in rows)
                {
                    if (row.RowIndex.Value == 1)
                    {
                        foreach (Cell cell in row.Descendants<Cell>())
                        {
                            DT.Columns.Add(GetExcelSheetCellValue(doc, cell, true));
                        }
                    }
                    else
                    {
                        DT.Rows.Add();
                        int i = 0;
                        foreach (Cell cell in row.Descendants<Cell>())
                        {
                            DT.Rows[DT.Rows.Count - 1][i] = GetExcelSheetCellValue(doc, cell, false);
                            i++;
                        }
                    }
                }
            }
            return DT;
        }

        private string GetExcelSheetCellValue(SpreadsheetDocument doc, Cell cell, bool Header)
        {
            string value = "";
            try
            {

                if (cell != null && cell.CellValue != null && cell.CellValue.InnerText != null)
                {
                    value = cell.CellValue.InnerText;
                    if (cell.CellReference.InnerText.Contains("K") || cell.CellReference.InnerText.Contains("L"))
                    {
                        if (!Header)
                            value = TimeSpan.FromDays(Convert.ToDouble(value)).ToString(@"hh\:mm");
                    }
                    if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
                    {
                        return doc.WorkbookPart.SharedStringTablePart.SharedStringTable.ChildElements.GetItem(int.Parse(value)).InnerText;
                    }
                }
                return value;
            }
            catch
            {
                return value;
            }
        }

        public string BindStringToHTML(string Data, char Delimiter, string HTMLTag)
        {
            StringBuilder Returned = new StringBuilder();
            string[] Array = Data.Split(Delimiter);
            foreach (string item in Array)
            {
                Returned.Append(string.Format("<{0}>{1}</{0}>", HTMLTag, item));
            }
            return Returned.ToString();
        }

        public DBExecResult DT2XML(DataTable DT, string RootName, string ChildName, out XmlDocument outXmlDoc)
        {
            DBExecResult retObj = new DBExecResult();
            try
            {
                xmlDoc = new XmlDocument();
                rootNode = xmlDoc.CreateElement(RootName);
                xmlDoc.AppendChild(rootNode);
                foreach (DataRow row in DT.Rows)
                {
                    node = xmlDoc.CreateElement(ChildName);
                    foreach (DataColumn col in DT.Columns)
                    {
                        xmlAttribute = xmlDoc.CreateAttribute(col.ColumnName);
                        xmlAttribute.Value = row[col].ToString().Trim(' ');
                        node.Attributes.Append(xmlAttribute);
                    }
                    rootNode.AppendChild(node);
                }
                outXmlDoc = xmlDoc;
                retObj.ErrorCode = 0;
                retObj.ErrorException = null;
                return retObj;
            }
            catch (Exception ex)
            {
                outXmlDoc = null;
                retObj.ErrorCode = errorCode;
                retObj.ErrorException = ex.ToString();
                return retObj;
            }
        }
         
        public string GetApplicationPath()
        {
            return AppContext.BaseDirectory.Substring(0, AppContext.BaseDirectory.IndexOf("bin"));
        }

        public XmlDocument ReadXMLFile(string relativePath)
        {
            var appPath = GetApplicationPath();
            string xmlFilePath = Path.Combine(appPath, relativePath);
            string xmlString = File.ReadAllText(xmlFilePath);
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlString);
            return xml;
        }

        public DBExecResult CallSQLDB(DBExecParams dBExecParams)
        {
            bool isStoredProcedure = string.IsNullOrEmpty(dBExecParams.StoredProcedure) ? false : true;
            bool isSQLFile = string.IsNullOrEmpty(dBExecParams.SQLFilePath) ? false : true;
            if (isSQLFile)
                dBExecParams.Query = ExtractQueryFromSQLFile(dBExecParams);

            DBExecResult execResult = new DBExecResult();
            execResult.ResultSet = new DataSet();
            try
            {
                using (sqlConnection = new SqlConnection(dBExecParams.ConString))
                {
                    sqlConnection.InfoMessage += new SqlInfoMessageEventHandler((object sender, SqlInfoMessageEventArgs e) =>
                    {
                        SQLMessageHandler(sender, e, ref execResult);
                    });
                    using (sqlCommand = new SqlCommand(isStoredProcedure ? dBExecParams.StoredProcedure : dBExecParams.Query, sqlConnection))
                    {
                        sqlCommand.CommandType = isStoredProcedure ? CommandType.StoredProcedure : CommandType.Text;
                        if (!isSQLFile && dBExecParams.Parameters != null && dBExecParams.Parameters.Count > 0)
                        {
                            FillSQLParams(dBExecParams, ref sqlCommand);
                        }
                        sqlConnection.Open();
                        switch (dBExecParams.ExecType)
                        {
                            case DBExecType.ExecuteNonQuery:
                                execResult.AffectedRowsCount = sqlCommand.ExecuteNonQuery();
                                break;
                            case DBExecType.DataAdapter:
                                sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                                sqlDataAdapter.Fill(execResult.ResultSet);
                                sqlDataAdapter.Dispose();
                                break;
                            case DBExecType.ExecuteScalar:
                                execResult.ResultField = sqlCommand.ExecuteScalar().ToString();
                                break;
                        }
                        sqlConnection.Close();
                    }
                }
                execResult.ErrorCode = 0;
                execResult.ErrorException = null;
                return execResult;
            }
            catch (Exception ex)
            {
                execResult.AffectedRowsCount = 0;
                execResult.ErrorCode = errorCode;
                execResult.ErrorException = $"ADO.NET Exception: {ex.ToString()}";
                Log($"//_-_\\ {dBExecParams.StoredProcedure} {dBExecParams.Query}: {execResult.ErrorException}");
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
                    if (CustomParams.Contains(param.Key))
                    {
                        byte[] ParamValue = ConvertFileBase64StringToByteArray(param.Value);
                        sqlCommand.Parameters.Add(new SqlParameter(param.Key, SqlDbType.VarBinary, ParamValue.Length)).Value = ParamValue;
                    }
                    else
                        sqlCommand.Parameters.Add(new SqlParameter(param.Key, param.Value));
                }
            }
        }

        private void SQLMessageHandler(object sender, SqlInfoMessageEventArgs e, ref DBExecResult returnedData)
        {
            // This gets all the messages generated during the execution of the SQL, 
            // including low-severity error messages.
            foreach (SqlError err in e.Errors)
            {
                returnedData.ExecutionMessages += $"  // $$ // {err.Procedure} line: {err.LineNumber} >> {err.Message}";
            }
        }

        public string SpreadWord(string Word)
        {
            string[] TempWord = new string[Word.Length * 2];
            int counter = 0;
            foreach (var ch in Word)
            {
                TempWord[counter] = ch + "_";
                counter++;
            }
            return TempWord.ToString();
        }

        public string ConvertImageURLToBase64(string url)
        {
            StringBuilder _sb = new StringBuilder();
            Byte[] _byte = GetImageURLAsBytes(url);

            _sb.Append(Convert.ToBase64String(_byte, 0, _byte.Length));

            return _sb.ToString();
        }

        private byte[] GetImageURLAsBytes(string url)
        {
            Stream stream = null;
            byte[] buf;

            WebProxy myProxy = new WebProxy();
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)req.GetResponse();
            stream = response.GetResponseStream();
            using (BinaryReader br = new BinaryReader(stream))
            {
                int len = (int)(response.ContentLength);
                buf = br.ReadBytes(len);
                br.Close();
            }

            stream.Close();
            response.Close();
            return (buf);
        }

        public bool DeletePhysicalFile(string ServerPathFileName)
        {
            string filePath = HttpContext.Current.Server.MapPath(ServerPathFileName);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
                return true;
            }
            return false;
        }

        public bool SaveBase64FileToServer(string ServerPath, string FileName, string FileData)
        {
            if (FileData.Contains("base64,"))
            {
                String path = HttpContext.Current.Server.MapPath(ServerPath);
                if (!System.IO.Directory.Exists(path))
                    System.IO.Directory.CreateDirectory(path);

                string filePhysicalPath = Path.Combine(path, FileName);
                FileData = FileData.Split(new string[] { "base64," }, StringSplitOptions.None)[1];
                FileData = FileData.Replace('-', '+').Replace('_', '/');
                byte[] imageBytes = Convert.FromBase64String(FileData);
                File.WriteAllBytes(filePhysicalPath, imageBytes);
                return true;
            }
            return false;
        }

        public DBExecResult SendMail(SendMailData mailData)
        {
            DBExecResult retObj = new DBExecResult();
            //Dictionary<string, string> dcGeneralMailSettings = GetLookups("GeneralMailSettings");
            //mailData.MailBody = dcGeneralMailSettings["SendMailBodyPrefix"] + mailData.MailBody + dcGeneralMailSettings["SendMailBodySuffix"];

            MailMessage msg = new MailMessage();
            try
            {
                msg.From = new MailAddress(mailData.SenderMail, mailData.SenderName);

                string[] Recievers = mailData.RecieverMail.Split(',');
                for (int i = 0; i < Recievers.Length; i++)
                {
                    msg.To.Add(Recievers[i]);
                }

                if (!string.IsNullOrEmpty(mailData.CCs))
                {
                    string[] CCs = mailData.CCs.Split(',');
                    for (int i = 0; i < CCs.Length; i++)
                    {
                        msg.CC.Add(CCs[i]);
                    }
                }

                if (!string.IsNullOrEmpty(mailData.AttFilePath) && mailData.AttFilePath != "ExceptionMail")
                {
                    string[] Attachments = mailData.AttFilePath.Split(',');
                    for (int i = 0; i < Attachments.Length; i++)
                    {
                        Attachment attachment = new Attachment(Attachments[i], MediaTypeNames.Application.Octet);
                        ContentDisposition disposition = attachment.ContentDisposition;
                        disposition.CreationDate = File.GetCreationTime(Attachments[i]);
                        disposition.ModificationDate = File.GetLastWriteTime(Attachments[i]);
                        disposition.ReadDate = File.GetLastAccessTime(Attachments[i]);
                        disposition.FileName = Path.GetFileName(Attachments[i]);
                        disposition.Size = new FileInfo(Attachments[i]).Length;
                        disposition.DispositionType = DispositionTypeNames.Attachment;
                        msg.Attachments.Add(attachment);
                    }
                }

                msg.Subject = mailData.MailSubject;
                msg.IsBodyHtml = true;
                msg.Body = HttpUtility.UrlDecode(mailData.MailBody);
                SmtpClient client = new SmtpClient();
                client.UseDefaultCredentials = true;
                //client.Host = dcGeneralMailSettings["SendMailSMTPClientHost"];
                //client.Port = Convert.ToInt32(dcGeneralMailSettings["SendMailSMTPClientPort"]);
                //client.EnableSsl = Convert.ToBoolean(Convert.ToInt32(dcGeneralMailSettings["SendMailIsSSLEnabled"]));
                client.DeliveryMethod = SmtpDeliveryMethod.Network;

                if (mailData.SenderPassword != "")
                {
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(msg.From.User, mailData.SenderPassword);
                }

                //client.Timeout = Convert.ToInt32(dcGeneralMailSettings["SendMailSMTPClientTimeout"]);

                client.Send(msg);
                retObj.ErrorCode = 0;
                retObj.ErrorException = null;
                LogMail(mailData, true, "");
                return retObj;
            }
            catch (Exception ex)
            {
                LogMail(mailData, false, ex.ToString());
                retObj.ErrorCode = errorCode;
                retObj.ErrorException = ex.ToString();
                return retObj;
            }
            finally
            {
                msg.Dispose();
            }
        }

        public void LogMail(SendMailData mailData, bool isSent, string exception)
        {
            try
            {
                if (mailData.AttFilePath != "ExceptionMail")
                {
                    Dictionary<string, string> parameters = new Dictionary<string, string>();
                    parameters.Add("@Attachments", string.IsNullOrEmpty(mailData.AttFilePath) ? "" : mailData.AttFilePath);
                    parameters.Add("@MailCC", string.IsNullOrEmpty(mailData.CCs) ? "" : mailData.CCs);
                    parameters.Add("@MailBody", string.IsNullOrEmpty(mailData.MailBody) ? "" : mailData.MailBody);
                    parameters.Add("@MailSubject", string.IsNullOrEmpty(mailData.MailSubject) ? "" : mailData.MailSubject);
                    parameters.Add("@MailTo", string.IsNullOrEmpty(mailData.RecieverMail) ? "" : mailData.RecieverMail);
                    parameters.Add("@MailFrom", string.IsNullOrEmpty(mailData.SenderMail) ? "" : mailData.SenderMail);
                    parameters.Add("@IsSent", isSent ? "1" : "0");
                    parameters.Add("@Exception", string.IsNullOrEmpty(exception) ? "No Exception" : exception);
                    CallSQLDB(new DBExecParams() { ConString = "", StoredProcedure = "spAppLogsMailLog", Parameters = parameters, ExecType = DBExecType.ExecuteNonQuery });
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }
        }

        public void SendExceptionMail(string ExceptionFiredFrom, string ExceptionDetails)
        {
            //SendMailData mailData = new SendMailData();
            //Dictionary<string, string> dcExceptionMailSettings = GetLookups("ExceptionMailSettings");
            //mailData.MailBody = dcExceptionMailSettings["ExceptionMailBody"].Replace("#*FirstName*#", dcExceptionMailSettings["ExceptionReceiverName"]).Replace("#*ExceptionFiredFrom*#", ExceptionFiredFrom).Replace("#*ExceptionDetails*#", ExceptionDetails);
            //mailData.AttFilePath = "ExceptionMail"; // used to detect that this mail is Exception mail so it will not logged.
            //mailData.MailSubject = dcExceptionMailSettings["ExceptionMailSubject"].Replace("#*ExceptionFiredFrom*#", ExceptionFiredFrom);
            //mailData.RecieverMail = dcExceptionMailSettings["ExceptionReceiverMail"];
            //mailData.RecieverName = dcExceptionMailSettings["ExceptionReceiverName"];
            //mailData.SenderMail = dcExceptionMailSettings["ExceptionSenderMail"];
            //mailData.SenderName = dcExceptionMailSettings["ExceptionSenderName"];
            //mailData.SenderPassword = dcExceptionMailSettings["ExceptionSenderPassword"];
            //SendMail(mailData);
        }

        //public void ShowJSAlert(System.Web.UI.Page Page, string Message)
        //{
        //    string script = "alert(\"" + Message + "\");";
        //    ScriptManager.RegisterStartupScript(Page, GetType(), "ServerControlScript", script, true);
        //}

        public byte[] ConvertFileBase64StringToByteArray(string File)
        {
            string[] FileData = File.Split(',');
            string FileString = FileData[1];
            //return Convert.FromBase64String(FileString);
            return Encoding.UTF8.GetBytes(FileString);
        }

        public object MapObjects(JObject objSource, object objDestination)
        {
            Type objDestinationType = objDestination.GetType();
            var objDestinationPropInfo = objDestinationType.GetProperties();
            foreach (PropertyInfo destProp in objDestinationPropInfo)
            {
                var srcPropValue = objSource[destProp.Name];
                if (objSource[destProp.Name] != null && srcPropValue != null)
                    destProp.SetValue(objDestination, Convert.ChangeType(srcPropValue, destProp.PropertyType), null);
            }
            return objDestination;
        }

        public DestinationType MapObjects<SourceType, DestinationType>(SourceType objSource)
        {
            DestinationType objDestination = CreateInstance<DestinationType>();
            var objDestinationPropInfo = typeof(DestinationType).GetProperties();
            Type objSourceType = typeof(SourceType);
            foreach (PropertyInfo destProp in objDestinationPropInfo)
            {
                if (objSourceType.GetProperty(destProp.Name) != null)
                {
                    var srcPropValue = objSourceType.GetProperty(destProp.Name).GetValue(objSource, null);
                    if (objSourceType.GetProperty(destProp.Name) != null && srcPropValue != null)
                    {
                        srcPropValue = srcPropValue is Boolean ? Convert.ToString(srcPropValue) : srcPropValue.ToString();
                        destProp.SetValue(objDestination, Convert.ChangeType(srcPropValue, destProp.PropertyType), null);
                    }
                }
            }
            return objDestination;
        }

        public void RecycleAppPool(string AppPool)
        {
            ServerManager serverManager = new ServerManager();
            ApplicationPool appPool = serverManager.ApplicationPools[AppPool];
            if (appPool != null)
            {
                if (appPool.State == ObjectState.Stopped)
                {
                    appPool.Start();
                }
                else
                {
                    appPool.Recycle();
                }
            }
        }

        public string CalculateCurrenyRate(string Amount, decimal FromRate, decimal ToRate)
        {
            if (ToRate <= 0 || FromRate <= 0)
                return "NA";
            else
            {
                string ReturnedValue = (FromRate * Convert.ToDecimal(Amount) / ToRate).ToString();
                if (ReturnedValue.Contains("."))
                    ReturnedValue = ReturnedValue.TrimStart(new Char[] { '0' }).TrimEnd(new Char[] { '0' });
                else
                    ReturnedValue = ReturnedValue.TrimStart(new Char[] { '0' });

                return ReturnedValue;
            }
        }

        public void AppendTextToFile(string text)
        {
            logsFileStream = new FileStream(LogFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
            logsStreamWriter = new StreamWriter(logsFileStream, Encoding.UTF8, 4096, true);
            logsStreamWriter.BaseStream.Seek(0, SeekOrigin.End);
            logsStreamWriter.WriteLineAsync(text);
            logsStreamWriter.Flush();
            logsStreamWriter.Close();
        }

        public Dictionary<string, string> ConvertToDictionary(DataTable DT)
        {
            return DT.AsEnumerable().ToDictionary(row => row.Field<string>(0), row => row.Field<string>(1));
        }

        public MonthName FormatMonth(int Month)
        {
            MonthName retMonth = new MonthName();
            List<Tuple<int, string, string>> Months = new List<Tuple<int, string, string>>()
            {
                new Tuple<int, string, string>(1, "January", "يناير"),
                new Tuple<int, string, string>(2, "February", "فبراير"),
                new Tuple<int, string, string>(3, "March", "مارس"),
                new Tuple<int, string, string>(4, "April", "أبريل"),
                new Tuple<int, string, string>(5, "May", "مايو"),
                new Tuple<int, string, string>(6, "June", "يونيو"),
                new Tuple<int, string, string>(7, "July", "يوليو"),
                new Tuple<int, string, string>(8, "August", "أغسطس"),
                new Tuple<int, string, string>(9, "September", "سبتمبر"),
                new Tuple<int, string, string>(10, "October", "أكتوبر"),
                new Tuple<int, string, string>(11, "November", "نوفمبر"),
                new Tuple<int, string, string>(12, "December", "ديسمبر")
            };
            var MonthName = Months.FirstOrDefault(x => x.Item1 == Month);
            retMonth.MonthName_EN = MonthName.Item2;
            retMonth.MonthName_AR = MonthName.Item3;
            return retMonth;
        }

        public string FormatBase64String(string Text)
        {
            if (!string.IsNullOrEmpty(Text))
            {
                string[] arrText = Text.Split(',');
                return (arrText[1] != null ? arrText[1] : arrText[0]).Replace(" ", "+");
            }
            return "";
        }

        public string GetBase64Extension(string Text)
        {
            if (!string.IsNullOrEmpty(Text))
            {
                string[] arrText = Text.Split(',');
                if (arrText[0] != null)
                {
                    var Mime = arrText[0].Split('/');
                    if (Mime[1] != null)
                        return Mime[1];
                }
            }
            return "";
        }

        public string GetCurrentIP()
        {
            return HttpContext.Current.Request.UserHostAddress;
        }

        public string MaskCardNumber(string CardNumber)
        {
            return string.Format("{0}******{1}", CardNumber.Trim().Substring(0, 6), CardNumber.Trim().Substring(12, 4));
        }

        public string Decrypt(string cipherText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        public void EncryptKeyInAppSetting(string Key)
        {
            string WebConfigPath = HttpContext.Current.Server.MapPath("~/Web.Config");
            XmlDocument doc = new XmlDocument();
            doc.Load(WebConfigPath);
            XmlNodeList list = doc.DocumentElement.SelectNodes(string.Format("appSettings/add[@key='{0}']", Key));

            if (list.Count == 1)
            {
                XmlNode node = list[0];
                string value = node.Attributes["value"].Value;
                node.Attributes["value"].Value = Encrypt(value);
                doc.Save(WebConfigPath);
            }
        }

        public string Encrypt(string clearText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        public string GetAppPhysicalPath()
        {
            return System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
        }

        public void Log(string logMessage)
        {
            StackTrace stackTrace = new StackTrace();
            string methodName = stackTrace.GetFrame(1).GetMethod().Name;
            logMessage = $"{DateTime.Now.ToString("hh.mm.ss.ffffff")} : {methodName} >> {logMessage}";
            AppendTextToFile(logMessage);
        }

        public void ManualLog(string message, XmlDocument xmlDocument = null)
        {
            message = $"{DateTime.Now.ToString("hh.mm.ss.ffffff")} >> {message}";
            if (xmlDocument != null)
            {
                using (var stringWriter = new StringWriter())
                using (var xmlTextWriter = XmlWriter.Create(stringWriter))
                {
                    xmlDocument.WriteTo(xmlTextWriter);
                    xmlTextWriter.Flush();
                    message += $" // XML // {stringWriter.GetStringBuilder().ToString()}";
                }
            }
            FileStream logsFileStream = new FileStream(@"C:\Inetpub\temLog.txt", FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
            StreamWriter logsStreamWriter = new StreamWriter(logsFileStream, System.Text.Encoding.UTF8, 4096, true);
            logsStreamWriter.BaseStream.Seek(0, SeekOrigin.End);
            logsStreamWriter.WriteLineAsync(message);
            logsStreamWriter.Flush();
            logsStreamWriter.Close();
        }
    }

    public class VirtualXML
    {
        string _rootNode;
        StringBuilder vDocument;
        public VirtualXML(string rootNode)
        {
            _rootNode = rootNode;
            vDocument = new StringBuilder($"<{rootNode}>");
        }
        public void AddNode(string nodeName, Dictionary<string, string> nodeAttributes)
        {
            string nodeAttr = string.Join(" ", nodeAttributes.Select(pair => $"{pair.Key}='{pair.Value.Trim()}' ").ToArray());
            vDocument.Append($"<{nodeName} {nodeAttr}/>");
        }
        public string Get()
        {
            vDocument.Append($"</{_rootNode}>");
            return vDocument.ToString();
        }
    }

    public class MonthName
    {
        public string MonthName_EN;
        public string MonthName_AR;
    }

    public enum DBExecType
    {
        ExecuteNonQuery = 1,
        ExecuteScalar = 2,
        DataAdapter = 3
    }

    public class DBExecParams
    {
        public string ConString { get; set; }
        public string StoredProcedure { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
        public DBExecType ExecType { get; set; }
        public string Query { get; set; }
        public List<string> WordsToDeleteFromSQLFile { get; set; }
        public List<string> WordsToKeepInSQLFile { get; set; }
        public string KeywordToSetParamsValue { get; set; }
        public string SQLFilePath { get; set; }
    }

    public class DBExecResult
    {
        public int ErrorCode { get; set; }
        public string ErrorException { get; set; }
        public string ExecutionMessages { get; set; }
        public DataSet ResultSet { get; set; }
        public string ResultField { get; set; }
        public int AffectedRowsCount { get; set; }
        public XmlDocument SerializeToXML()
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlSerializer xsData = new XmlSerializer(typeof(DBExecResult));
            var returnedXML = "";

            using (var SW = new StringWriter())
            {
                using (XmlWriter XW = XmlWriter.Create(SW))
                {
                    xsData.Serialize(XW, this);
                    returnedXML = SW.ToString(); // Your XML
                }
            }
            xmlDoc.LoadXml(returnedXML);
            return xmlDoc;
        }
    }

    public class SendMailData
    {
        public string SenderName { get; set; }
        public string SenderMail { get; set; }
        public string SenderPassword { get; set; }
        public string SMTPClientHost { get; set; }
        public int SMTPClientPort { get; set; }
        public bool IsSSLEnabled { get; set; }
        public int SMTPClientTimeout { get; set; }
        public string RecieverName { get; set; }
        public string RecieverMail { get; set; }
        public string CCs = "";
        public string MailSubject { get; set; }
        public string AttFilePath = "";
        public string MailBody { get; set; }
    }

    public class CommonProps
    {
        public string FailedAttibute { get; set; }
    }
}
