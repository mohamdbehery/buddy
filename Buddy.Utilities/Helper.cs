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
        private XmlNode RootNode;
        private XmlNode Node;
        private XmlAttribute Attr;

        private DataTable DT1;
        private DataSet DS1;
        private SqlConnection SQLCon;
        private SqlCommand SQLCMD;
        private SqlDataAdapter SQLDataAd;
        readonly FileStream logsFileStream;
        readonly StreamWriter logsStreamWriter;

        public Helper(bool withLogs)
        {
            logsFileStream = new FileStream(LogFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
            logsStreamWriter = new StreamWriter(logsFileStream, Encoding.UTF8, 4096, true);
        }

        public Helper()
        {

        }

        public string LogFilePath {
            get{
                int fileMaxSizeInBytes = 2000000;
                string logFileExtension = ".txt";
                string LogsBaseDirectory = @"C:\Inetpub";
                string LogFileNamePrefix = "Log";
                string LogsDirectory = Path.Combine(LogsBaseDirectory, "BuddyLogger");
                StackTrace stackTrace = new StackTrace();
                string projectName = "UnknownSource";
                try
                {
                    string assemblyName = Assembly.GetEntryAssembly().ManifestModule.Name;
                    projectName = assemblyName.Remove(assemblyName.IndexOf('.'));
                }
                catch { }

                if (!Directory.Exists(LogsDirectory))
                    Directory.CreateDirectory(LogsDirectory);

                string crDate = DateTime.Now.ToString("MM-dd-yyyy");
                string DayLogsDirectory = Path.Combine(LogsDirectory, crDate);
                if (!Directory.Exists(DayLogsDirectory))
                    Directory.CreateDirectory(DayLogsDirectory);

                string ProjectDirectory = Path.Combine(DayLogsDirectory, projectName);
                if (!Directory.Exists(ProjectDirectory))
                    Directory.CreateDirectory(ProjectDirectory);

                string crHour = DateTime.Now.Hour.ToString();
                string hourLogsFilePath = Path.Combine(ProjectDirectory, $"{LogFileNamePrefix}-{crDate}-{crHour}-1{logFileExtension}");

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

        private string[] CustomParams = { "@HRPersonalPhoto", "@FollowUpAttachmentFilePath" };

        public string GetConfigKey(string Key)
        {
            return ConfigurationManager.ConnectionStrings[Key].ToString();
        }

        public string GetAppKey(string Key)
        {
            return ConfigurationManager.AppSettings[Key].ToString();
        }

        public string GetSubString(string Content, string From, string To)
        {
            int FromIndex = Content.IndexOf(From) + From.Length;
            int ToIndex = Content.LastIndexOf(To);
            return Content.Substring(FromIndex, ToIndex - FromIndex);
        }

        public string RemoveSubString(string Content, string From, string To)
        {
            if (Content.Contains(From) && Content.Contains(To))
            {
                int FromIndex = Content.IndexOf(From);
                int ToIndex = Content.IndexOf(To, FromIndex);
                return Content.Remove(FromIndex, ((ToIndex + To.Length) - FromIndex));
            }
            else
                return Content;
        }

        public string BindStringAttributes(string Content, XmlNode Source)
        {
            foreach (XmlAttribute Item in Source.Attributes)
            {
                if (Content.Contains("#" + Item.Name.ToString() + "#"))
                    Content = Content.Replace("#" + Item.Name.ToString() + "#", Item.Value);
            }
            return Content;
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

        public ReturnedData DT2XML(DataTable DT, string RootName, string ChildName, out XmlDocument outXmlDoc)
        {
            ReturnedData retObj = new ReturnedData();
            try
            {
                xmlDoc = new XmlDocument();
                RootNode = xmlDoc.CreateElement(RootName);
                xmlDoc.AppendChild(RootNode);
                foreach (DataRow row in DT.Rows)
                {
                    Node = xmlDoc.CreateElement(ChildName);
                    foreach (DataColumn col in DT.Columns)
                    {
                        Attr = xmlDoc.CreateAttribute(col.ColumnName);
                        Attr.Value = row[col].ToString().Trim(' ');
                        Node.Attributes.Append(Attr);
                    }
                    RootNode.AppendChild(Node);
                }
                outXmlDoc = xmlDoc;
                retObj.errorCode = 0;
                retObj.errorException = null;
                return retObj;
            }
            catch (Exception ex)
            {
                outXmlDoc = null;
                retObj.errorCode = errorCode;
                retObj.errorException = ex.ToString();
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

        public ReturnedData GetDSFromSQLDB_SP(string ConString, string SP, Dictionary<string, string> Params, out DataSet outDS)
        {
            ReturnedData retObj = new ReturnedData();
            try
            {
                DS1 = new DataSet();
                using (SqlConnection SQLCon = new SqlConnection(ConString))
                {
                    using (SqlCommand SQLCMD = new SqlCommand(SP, SQLCon))
                    {
                        SQLCMD.CommandType = CommandType.StoredProcedure;
                        if (Params != null && Params.Count > 0)
                        {
                            foreach (var Param in Params)
                            {
                                if (string.IsNullOrEmpty(Param.Value))
                                    SQLCMD.Parameters.Add(new SqlParameter(Param.Key, DBNull.Value));
                                else
                                {
                                    if (CustomParams.Contains(Param.Key))
                                    {
                                        byte[] ParamValue = ConvertFileBase64StringToByteArray(Param.Value);
                                        SQLCMD.Parameters.Add(new SqlParameter(Param.Key, SqlDbType.VarBinary, ParamValue.Length)).Value = ParamValue;
                                    }
                                    else
                                        SQLCMD.Parameters.Add(new SqlParameter(Param.Key, Param.Value));
                                }
                            }
                        }
                        SQLCon.Open();
                        SQLDataAd = new SqlDataAdapter(SQLCMD);
                        SQLDataAd.Fill(DS1);

                        SQLCon.Close();
                        SQLDataAd.Dispose();
                    }
                }
                outDS = DS1;
                retObj.errorCode = 0;
                retObj.errorException = null;
                return retObj;
            }
            catch (Exception ex)
            {
                outDS = null;
                retObj.errorCode = errorCode;
                retObj.errorException = ex.ToString();
                return retObj;
            }
        }

        public ReturnedData GetDTFromSQLDB_SP(string ConString, string SP, Dictionary<string, string> Params, out DataTable outDT)
        {
            ReturnedData retObj = new ReturnedData();
            try
            {
                DT1 = new DataTable();
                using (SqlConnection SQLCon = new SqlConnection(ConString))
                {
                    using (SqlCommand SQLCMD = new SqlCommand(SP, SQLCon))
                    {
                        SQLCMD.CommandType = CommandType.StoredProcedure;
                        if (Params != null && Params.Count > 0)
                        {
                            foreach (var Param in Params)
                            {
                                if (string.IsNullOrEmpty(Param.Value))
                                    SQLCMD.Parameters.Add(new SqlParameter(Param.Key, DBNull.Value));
                                else
                                {
                                    if (CustomParams.Contains(Param.Key))
                                    {
                                        byte[] ParamValue = ConvertFileBase64StringToByteArray(Param.Value);
                                        SQLCMD.Parameters.Add(new SqlParameter(Param.Key, SqlDbType.VarBinary, ParamValue.Length)).Value = ParamValue;
                                    }
                                    else
                                        SQLCMD.Parameters.Add(new SqlParameter(Param.Key, Param.Value));
                                }
                            }
                        }
                        SQLCon.Open();
                        SQLDataAd = new SqlDataAdapter(SQLCMD);
                        SQLDataAd.Fill(DT1);

                        SQLCon.Close();
                        SQLDataAd.Dispose();
                    }
                }
                outDT = DT1;
                retObj.errorCode = 0;
                retObj.errorException = null;
                return retObj;
            }
            catch (Exception ex)
            {
                outDT = null;
                retObj.errorCode = errorCode;
                retObj.errorException = ex.ToString();
                return retObj;
            }
        }

        public ReturnedData GetDSFromSQLDB_Query(string ConString, string Query, Dictionary<string, string> Params, out DataSet outDS)
        {
            ReturnedData retObj = new ReturnedData();
            try
            {
                DS1 = new DataSet();
                SQLCon = new SqlConnection(ConString);
                SQLCMD = new SqlCommand(Query, SQLCon);
                if (Params != null && Params.Count > 0)
                {
                    foreach (var Param in Params)
                    {
                        if (string.IsNullOrEmpty(Param.Value))
                            SQLCMD.Parameters.Add(new SqlParameter(Param.Key, DBNull.Value));
                        else
                        {
                            if (CustomParams.Contains(Param.Key))
                            {
                                byte[] ParamValue = ConvertFileBase64StringToByteArray(Param.Value);
                                SQLCMD.Parameters.Add(new SqlParameter(Param.Key, SqlDbType.VarBinary, ParamValue.Length)).Value = ParamValue;
                            }
                            else
                                SQLCMD.Parameters.Add(new SqlParameter(Param.Key, Param.Value));
                        }
                    }
                }
                SQLCon.Open();
                SQLDataAd = new SqlDataAdapter(SQLCMD);
                SQLDataAd.Fill(DS1);

                SQLCon.Close();
                SQLDataAd.Dispose();
                outDS = DS1;
                retObj.errorCode = 0;
                retObj.errorException = null;
                return retObj;
            }
            catch (Exception ex)
            {
                outDS = null;
                retObj.errorCode = errorCode;
                retObj.errorException = ex.ToString();
                return retObj;
            }
        }

        public ReturnedData GetDTFromSQLDB_Query(string ConString, string Query, Dictionary<string, string> Params, out DataTable outDT)
        {
            ReturnedData retObj = new ReturnedData();
            try
            {
                DT1 = new DataTable();
                SQLCon = new SqlConnection(ConString);
                SQLCMD = new SqlCommand(Query, SQLCon);
                if (Params != null && Params.Count > 0)
                {
                    foreach (var Param in Params)
                    {
                        if (string.IsNullOrEmpty(Param.Value))
                            SQLCMD.Parameters.Add(new SqlParameter(Param.Key, DBNull.Value));
                        else
                        {
                            if (CustomParams.Contains(Param.Key))
                            {
                                byte[] ParamValue = ConvertFileBase64StringToByteArray(Param.Value);
                                SQLCMD.Parameters.Add(new SqlParameter(Param.Key, SqlDbType.VarBinary, ParamValue.Length)).Value = ParamValue;
                            }
                            else
                                SQLCMD.Parameters.Add(new SqlParameter(Param.Key, Param.Value));
                        }
                    }
                }
                SQLCon.Open();
                SQLDataAd = new SqlDataAdapter(SQLCMD);
                SQLDataAd.Fill(DT1);
                SQLCon.Close();
                SQLDataAd.Dispose();

                outDT = DT1;
                retObj.errorCode = 0;
                retObj.errorException = null;
                return retObj;
            }
            catch (Exception ex)
            {
                outDT = null;
                retObj.errorCode = errorCode;
                retObj.errorException = ex.ToString();
                return retObj;
            }
        }

        public ReturnedData ExecuteSQLDB_SP(string ConStr, string SP, Dictionary<string, string> Params, out int AffectedRows)
        {
            ReturnedData retObj = new ReturnedData();
            try
            {
                int Rows = 0;
                using (SQLCon = new SqlConnection(ConStr))
                {
                    using (SQLCMD = new SqlCommand(SP, SQLCon))
                    {
                        SQLCMD.CommandType = CommandType.StoredProcedure;
                        if (Params != null && Params.Count > 0)
                        {
                            foreach (var Param in Params)
                            {
                                if (string.IsNullOrEmpty(Param.Value))
                                    SQLCMD.Parameters.Add(new SqlParameter(Param.Key, DBNull.Value));
                                else
                                {
                                    if (CustomParams.Contains(Param.Key))
                                    {
                                        byte[] ParamValue = ConvertFileBase64StringToByteArray(Param.Value);
                                        SQLCMD.Parameters.Add(new SqlParameter(Param.Key, SqlDbType.VarBinary, ParamValue.Length)).Value = ParamValue;
                                    }
                                    else
                                        SQLCMD.Parameters.Add(new SqlParameter(Param.Key, Param.Value));
                                }
                            }
                        }
                        SQLCon.Open();
                        Rows = SQLCMD.ExecuteNonQuery();
                        SQLCon.Close();
                    }
                }
                AffectedRows = Rows;
                retObj.affectedRows = AffectedRows;
                retObj.errorCode = 0;
                retObj.errorException = null;
                return retObj;
            }
            catch (Exception ex)
            {
                AffectedRows = 0;
                retObj.errorCode = errorCode;
                retObj.errorException = ex.ToString();
                return retObj;
            }
        }

        public ReturnedData ExecuteSQLDB_SP(string ConStr, string SP, Dictionary<string, string> Params, out string ReturnedField)
        {
            ReturnedData retObj = new ReturnedData();
            try
            {
                string FieldValue = "";
                using (SQLCon = new SqlConnection(ConStr))
                {
                    using (SQLCMD = new SqlCommand(SP, SQLCon))
                    {
                        SQLCMD.CommandType = CommandType.StoredProcedure;
                        if (Params != null && Params.Count > 0)
                        {
                            foreach (var Param in Params)
                            {
                                if (string.IsNullOrEmpty(Param.Value))
                                    SQLCMD.Parameters.Add(new SqlParameter(Param.Key, DBNull.Value));
                                else
                                {
                                    if (CustomParams.Contains(Param.Key))
                                    {
                                        byte[] ParamValue = ConvertFileBase64StringToByteArray(Param.Value);
                                        SQLCMD.Parameters.Add(new SqlParameter(Param.Key, SqlDbType.VarBinary, ParamValue.Length)).Value = ParamValue;
                                    }
                                    else
                                        SQLCMD.Parameters.Add(new SqlParameter(Param.Key, Param.Value));
                                }
                            }
                        }
                        SQLCon.Open();
                        FieldValue = SQLCMD.ExecuteScalar().ToString();
                        SQLCon.Close();
                    }
                }
                ReturnedField = FieldValue;
                retObj.errorCode = 0;
                retObj.errorException = null;
                return retObj;
            }
            catch (Exception ex)
            {
                ReturnedField = "";
                retObj.errorCode = errorCode;
                retObj.errorException = ex.ToString();
                return retObj;
            }
        }

        public ReturnedData ExecuteSQLDB_Query(string ConStr, string Query, Dictionary<string, string> Params, out string ReturnedField)
        {
            ReturnedData retObj = new ReturnedData();
            try
            {
                string FieldValue = "";
                using (SQLCon = new SqlConnection(ConStr))
                {
                    using (SQLCMD = new SqlCommand(Query, SQLCon))
                    {
                        if (Params != null && Params.Count > 0)
                        {
                            foreach (var Param in Params)
                            {
                                if (string.IsNullOrEmpty(Param.Value))
                                    SQLCMD.Parameters.Add(new SqlParameter(Param.Key, DBNull.Value));
                                else
                                {
                                    if (CustomParams.Contains(Param.Key))
                                    {
                                        byte[] ParamValue = ConvertFileBase64StringToByteArray(Param.Value);
                                        SQLCMD.Parameters.Add(new SqlParameter(Param.Key, SqlDbType.VarBinary, ParamValue.Length)).Value = ParamValue;
                                    }
                                    else
                                        SQLCMD.Parameters.Add(new SqlParameter(Param.Key, Param.Value));
                                }
                            }
                        }
                        SQLCon.Open();
                        FieldValue = SQLCMD.ExecuteScalar().ToString();
                        SQLCon.Close();
                    }
                }
                ReturnedField = FieldValue;
                retObj.errorCode = 0;
                retObj.errorException = null;
                return retObj;
            }
            catch (Exception ex)
            {
                ReturnedField = "";
                retObj.errorCode = errorCode;
                retObj.errorException = ex.ToString();
                return retObj;
            }
        }

        public ReturnedData ExecuteSQLDB_Query(string ConStr, string Query, Dictionary<string, string> Params, out int AffectedRows)
        {
            ReturnedData retObj = new ReturnedData();
            try
            {
                int Rows = 0;
                using (SQLCon = new SqlConnection(ConStr))
                {
                    using (SQLCMD = new SqlCommand(Query, SQLCon))
                    {
                        if (Params != null && Params.Count > 0)
                        {
                            foreach (var Param in Params)
                            {
                                if (string.IsNullOrEmpty(Param.Value))
                                    SQLCMD.Parameters.Add(new SqlParameter(Param.Key, DBNull.Value));
                                else
                                {
                                    if (CustomParams.Contains(Param.Key))
                                    {
                                        byte[] ParamValue = ConvertFileBase64StringToByteArray(Param.Value);
                                        SQLCMD.Parameters.Add(new SqlParameter(Param.Key, SqlDbType.VarBinary, ParamValue.Length)).Value = ParamValue;
                                    }
                                    else
                                        SQLCMD.Parameters.Add(new SqlParameter(Param.Key, Param.Value));
                                }
                            }
                        }
                        SQLCon.Open();
                        Rows = SQLCMD.ExecuteNonQuery();
                        SQLCon.Close();
                    }
                }

                AffectedRows = Rows;
                retObj.errorCode = 0;
                retObj.errorException = null;
                return retObj;
            }
            catch (Exception ex)
            {
                AffectedRows = 0;
                retObj.errorCode = errorCode;
                retObj.errorException = ex.ToString();
                return retObj;
            }
        }

        public ReturnedData ExecuteSQLDB_File(string ConStr, string FilePath, Dictionary<string, string> Params, List<string> WordsToDeleteFromSQLFile, List<string> WordsToKeepInSQLFile, string KeywordToSetParamsValue, out DataTable outDT, out List<string> outMessages)
        {
            string Query = File.ReadAllText(FilePath).Replace("\n", " \n ");
            if (WordsToKeepInSQLFile != null)
            {
                foreach (var word in WordsToKeepInSQLFile)
                {
                    Query = Query.Replace(word, SpreadWord(word));
                }
            }
            //.Replace("\r", " ").Replace("GOOD", "MOOD").Replace("\t", " ").Replace("\n", " \n ").Replace("GO ", " ").Replace(" GO", " ").Replace("MOOD", "GOOD").Replace("USE ETX_DB", " ");
            if (WordsToDeleteFromSQLFile != null)
            {
                foreach (var word in WordsToDeleteFromSQLFile)
                {
                    Query = Query.Replace(word, " ");
                    Query = Query.Replace(" " + word + " ", " ");
                    Query = Query.Replace(" " + word, " ");
                    Query = Query.Replace(word + " ", " ");
                }
            }

            if (WordsToKeepInSQLFile != null)
            {
                foreach (var word in WordsToKeepInSQLFile)
                {
                    Query = Query.Replace(SpreadWord(word), word);
                }
            }

            List<string> SQLMessages = new List<string>();
            ReturnedData retObj = new ReturnedData();
            try
            {
                DT1 = new DataTable();
                SQLCon = new SqlConnection(ConStr);
                string ParamsValues = "";
                if (Params != null && Params.Count > 0)
                {
                    foreach (var Param in Params)
                    {
                        ParamsValues += " SET " + Param.Key + "=" + (!string.IsNullOrEmpty(Param.Value) ? (Param.Key.Contains("CODE") ? "'" + Param.Value + "'" : Param.Value) : "NULL; ");
                    }
                }

                if (!string.IsNullOrEmpty(KeywordToSetParamsValue))
                    Query = Query.Replace(KeywordToSetParamsValue, ParamsValues);

                SQLCMD = new SqlCommand(Query);
                SQLCMD.CommandType = CommandType.Text;
                SQLCon.Open();
                SQLCon.InfoMessage += delegate (object sender, SqlInfoMessageEventArgs e)
                {
                    SQLMessages.Add(e.Message);
                };
                SQLDataAd = new SqlDataAdapter(SQLCMD.CommandText.ToString(), SQLCon);
                SQLDataAd.Fill(DT1);
                SQLCon.Close();
                SQLDataAd.Dispose();

                outDT = DT1;
                outMessages = SQLMessages;
                retObj.errorCode = 0;
                retObj.errorException = null;
                return retObj;
            }
            catch (Exception ex)
            {
                outDT = null;
                retObj.errorCode = errorCode;
                retObj.errorException = ex.ToString();
                if (ex.InnerException is SqlException)
                    retObj.errorException += "  Query  : " + Query;
                outMessages = null;
                return retObj;
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

        public ReturnedData SendMail(SendMailData mailData)
        {
            ReturnedData retObj = new ReturnedData();
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
                retObj.errorCode = 0;
                retObj.errorException = null;
                LogMail(mailData, true, "");
                return retObj;
            }
            catch (Exception ex)
            {
                LogMail(mailData, false, ex.ToString());
                retObj.errorCode = errorCode;
                retObj.errorException = ex.ToString();
                return retObj;
            }
            finally
            {
                msg.Dispose();
            }
        }

        public void LogMail(SendMailData mailData, bool IsSent, string Exception)
        {
            try
            {
                if (mailData.AttFilePath != "ExceptionMail")
                {
                    Dictionary<string, string> Params = new Dictionary<string, string>();
                    Params.Add("@Attachments", string.IsNullOrEmpty(mailData.AttFilePath) ? "" : mailData.AttFilePath);
                    Params.Add("@MailCC", string.IsNullOrEmpty(mailData.CCs) ? "" : mailData.CCs);
                    Params.Add("@MailBody", string.IsNullOrEmpty(mailData.MailBody) ? "" : mailData.MailBody);
                    Params.Add("@MailSubject", string.IsNullOrEmpty(mailData.MailSubject) ? "" : mailData.MailSubject);
                    Params.Add("@MailTo", string.IsNullOrEmpty(mailData.RecieverMail) ? "" : mailData.RecieverMail);
                    Params.Add("@MailFrom", string.IsNullOrEmpty(mailData.SenderMail) ? "" : mailData.SenderMail);
                    Params.Add("@IsSent", IsSent ? "1" : "0");
                    Params.Add("@Exception", string.IsNullOrEmpty(Exception) ? "No Exception" : Exception);

                    int outAffectedRows = 0;
                    ExecuteSQLDB_SP("dr", "spAppLogsMailLog", Params, out outAffectedRows);
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
            DestinationType objDestination = (DestinationType)Activator.CreateInstance(typeof(DestinationType));
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
    }

    public class VirtualXML
    {
        string _rootNode;
        StringBuilder VDocument;
        public VirtualXML(string rootNode)
        {
            _rootNode = rootNode;
            VDocument = new StringBuilder($"<{rootNode}>");
        }
        public void AddNode(string nodeName, Dictionary<string, string> nodeAttributes)
        {
            string nodeAttr = string.Join(" ", nodeAttributes.Select(pair => $"{pair.Key}='{pair.Value}' ").ToArray());
            VDocument.Append($"<{nodeName} {nodeAttr}/>");
        }
        public string Get()
        {
            VDocument.Append($"</{_rootNode}>");
            return VDocument.ToString();
        }
    }

    public class MonthName
    {
        public string MonthName_EN;
        public string MonthName_AR;
    }

    public class ReturnedData
    {
        public int errorCode;
        public string errorException;
        public DataSet dataContainer;
        public int affectedRows;
        public XmlDocument SerializeToXML(ReturnedData _RetData)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlSerializer xsData = new XmlSerializer(typeof(ReturnedData));
            var ReturnedXML = "";

            using (var SW = new StringWriter())
            {
                using (XmlWriter XW = XmlWriter.Create(SW))
                {
                    xsData.Serialize(XW, _RetData);
                    ReturnedXML = SW.ToString(); // Your XML
                }
            }
            xmlDoc.LoadXml(ReturnedXML);
            return xmlDoc;
        }
    }

    public class SendMailData
    {
        public string SenderName;
        public string SenderMail;
        public string SenderPassword;
        public string SMTPClientHost;
        public int SMTPClientPort;
        public bool IsSSLEnabled;
        public int SMTPClientTimeout;
        public string RecieverName;
        public string RecieverMail;
        public string CCs = "";
        public string MailSubject;
        public string AttFilePath = "";
        public string MailBody;
    }

    public class CommonProps
    {
        public string FailedAttibute { get; set; }
    }
}
