using Buddy.Utilities.DB;
using Buddy.Utilities.Enums;
using Buddy.Utilities.Models;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Web.Administration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Xml;

namespace Buddy.Utilities
{
    public class Helper: HelperBase
    {
        private XmlDocument XMLDoc;
        private XmlNode RootNode;
        private XmlNode XMLNode;
        private XmlAttribute XMLAttribute;
        ExecResult execResult;
        public DBConsumer DBConsumer { get; private set; }
        public Logger Logger { get; private set; }

        public Helper() : this(true)
        {

        }
        public Helper(bool initDBConsumer) : this("")
        {
            this.DBConsumer = CreateInstance<DBConsumer>();
        }

        public Helper(string LogsDirectory)
        {
            this.Logger = new Logger(LogsDirectory);
        }

        public static Helper CreateInstance()
        {
            return new Helper();
        }
        public static T CreateInstance<T>() where T : class
        {
            T objectInstance = (T)Activator.CreateInstance(typeof(T));
            return objectInstance;
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

        public ExecResult DT2XML(DataTable DT, string RootName, string ChildName, out XmlDocument outXmlDoc)
        {
            execResult = new ExecResult();
            try
            {
                XMLDoc = new XmlDocument();
                RootNode = XMLDoc.CreateElement(RootName);
                XMLDoc.AppendChild(RootNode);
                foreach (DataRow row in DT.Rows)
                {
                    XMLNode = XMLDoc.CreateElement(ChildName);
                    foreach (DataColumn col in DT.Columns)
                    {
                        XMLAttribute = XMLDoc.CreateAttribute(col.ColumnName);
                        XMLAttribute.Value = row[col].ToString().Trim(' ');
                        XMLNode.Attributes.Append(XMLAttribute);
                    }
                    RootNode.AppendChild(XMLNode);
                }
                outXmlDoc = XMLDoc;
                return execResult;
            }
            catch (Exception ex)
            {
                outXmlDoc = null;
                execResult.ErrorCode = HelperEnums.ErrorCode.Exception;
                execResult.ErrorException = ex.ToString();
                return execResult;
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

        //public void ShowJSAlert(System.Web.UI.Page Page, string Message)
        //{
        //    string script = "alert(\"" + Message + "\");";
        //    ScriptManager.RegisterStartupScript(Page, GetType(), "ServerControlScript", script, true);
        //}

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

        // [Note] yeld return should return into IEnumerabe
        public IEnumerable<DestinationType> MapObjects<SourceType, DestinationType>(List<SourceType> objSourceList, bool hardUpdate = true) where SourceType : class where DestinationType : class
        {
            foreach (var objectSource in objSourceList)
            {
                yield return MapObjects<SourceType, DestinationType>(objectSource, hardUpdate);
            }
        }

        public DestinationType MapObjects<SourceType, DestinationType>(SourceType objSource, bool hardUpdate = true) where SourceType : class where DestinationType : class
        {
            // if hard update, it always replace values in destination, even the source is null
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
                        if(srcPropValue == null && !hardUpdate)
                        {
                            // do nothing
                        }
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
}
