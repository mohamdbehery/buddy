using Buddy.Utilities.Enums;
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
        private readonly FileStream LogFileStream;
        private readonly StreamWriter LogStreamWriter;
        string projectName = Constants.UnknownProjectName;
        string logsDirectory = "";
        int logFileMaxSizeInBytes = Constants.LogFileMaxSizeInBytes;
        public Logger()
        {
            LogFileStream = new FileStream(LogFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
            LogStreamWriter = new StreamWriter(LogFileStream, Encoding.UTF8, 4096, true);
        }

        /// <summary>
        /// ex: "C:\Inetpub\BuddyLogger"
        /// </summary>
        /// <param name="logDirectory"></param>
        public Logger(string logDirectory): this()
        {
            this.LogsDirectory = logDirectory;
        }

        public string LogsDirectory {
            get => logsDirectory;
            set => this.logsDirectory = value;
        }
        public string LogFilePath
        {
            get
            {
                string currentDate = DateTime.Now.ToString("MM-dd-yyyy");
                string currentHour = DateTime.Now.Hour.ToString();

                if (!string.IsNullOrEmpty(GetAppKey("LogsMaxFileSize")))
                {
                    int temp = 0;
                    if (int.TryParse(GetAppKey("LogsMaxFileSize"), out temp))
                        logFileMaxSizeInBytes = temp;
                }

                if (string.IsNullOrEmpty(LogsDirectory))
                {
                    if (!string.IsNullOrEmpty(GetAppKey("LogsDicrectory")))
                        LogsDirectory = GetAppKey("LogsDicrectory");
                    else
                        LogsDirectory = Constants.DefaultLogsDirectory;
                }
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
        public void ManualLog(string message, XmlDocument xmlDocument = null)
        {
            message = $"ManualLog {DateTime.Now.ToString("hh.mm.ss.ffffff")} >> {message}";
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
            FileStream logsFileStream = new FileStream(@"C:\Inetpub\tempLog.txt", FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
            StreamWriter logsStreamWriter = new StreamWriter(logsFileStream, System.Text.Encoding.UTF8, 4096, true);
            logsStreamWriter.BaseStream.Seek(0, SeekOrigin.End);
            logsStreamWriter.WriteLineAsync(message);
            logsStreamWriter.Flush();
            logsStreamWriter.Close();
        }
    }
}
