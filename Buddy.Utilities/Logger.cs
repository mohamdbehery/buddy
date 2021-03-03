using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Buddy.Utilities
{
    public class Logger: HelperBase
    {
        private FileStream LogFileStream;
        private StreamWriter LogStreamWriter;
        public Logger()
        {
            LogFileStream = new FileStream(LogFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
            LogStreamWriter = new StreamWriter(LogFileStream, Encoding.UTF8, 4096, true);
        }

        private string LogFilePath
        {
            get
            {
                string logsDirectory = Constants.DefaultLogsDirectory;
                int logFileMaxSizeInBytes = Constants.LogFileMaxSizeInBytes;
                string currentDate = DateTime.Now.ToString("MM-dd-yyyy");
                string currentHour = DateTime.Now.Hour.ToString();
                string projectName = Constants.UnknownProjectName;

                if (!string.IsNullOrEmpty(GetAppKey("LogsMaxFileSize")))
                {
                    int temp = 0;
                    if (int.TryParse(GetAppKey("LogsMaxFileSize"), out temp))
                        logFileMaxSizeInBytes = temp;
                }

                if (!string.IsNullOrEmpty(GetAppKey("LogsDicrectory")))
                    logsDirectory = GetAppKey("LogsDicrectory");

                StackTrace stackTrace = new StackTrace();
                try
                {
                    string assemblyName = Assembly.GetEntryAssembly().ManifestModule.Name;
                    projectName = assemblyName.Remove(assemblyName.IndexOf('.'));
                }
                catch { }

                if (!Directory.Exists(logsDirectory))
                    Directory.CreateDirectory(logsDirectory);

                string DayLogsDirectory = Path.Combine(logsDirectory, currentDate);
                if (!Directory.Exists(DayLogsDirectory))
                    Directory.CreateDirectory(DayLogsDirectory);

                string ProjectDirectory = Path.Combine(DayLogsDirectory, projectName);
                if (!Directory.Exists(ProjectDirectory))
                    Directory.CreateDirectory(ProjectDirectory);

                string hourLogsFilePath = Path.Combine(ProjectDirectory, $"{Constants.LogFileNamePrefix}-{currentDate}-{currentHour}-1{Constants.DefaultLogExtention}");

                if (File.Exists(hourLogsFilePath))
                {
                    FileInfo logFileInfo = new FileInfo(hourLogsFilePath);
                    if (logFileInfo.Length > logFileMaxSizeInBytes)
                    {
                        string logFileName = Path.GetFileNameWithoutExtension(hourLogsFilePath);
                        string[] logFileNameParts = logFileName.Split('-');
                        int fileVersion = 0;
                        if (int.TryParse(logFileNameParts[logFileNameParts.Length - 1], out fileVersion))
                        {
                            fileVersion++;
                            logFileNameParts[logFileNameParts.Length - 1] = fileVersion.ToString();
                            hourLogsFilePath = Path.Combine(ProjectDirectory, $"{string.Join("-", logFileNameParts)}{Constants.DefaultLogExtention}");
                        }
                    }
                }
                return hourLogsFilePath;
            }
        }
        public void Log(string logMessage)
        {
            StackTrace stackTrace = new StackTrace();
            string methodName = stackTrace.GetFrame(1).GetMethod().Name;
            logMessage = $"{DateTime.Now.ToString("hh.mm.ss.ffffff")} : {methodName} >> {logMessage}";
            
            LogStreamWriter.BaseStream.Seek(0, SeekOrigin.End);
            LogStreamWriter.WriteLineAsync(logMessage);
            LogStreamWriter.Flush();
            LogStreamWriter.Close();
        }
        public void AppendTextToFile(string text)
        {
            
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
}
