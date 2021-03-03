using Buddy.Utilities;
using Buddy.Utilities.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileWatcherWinService
{
    partial class FileWatcherService : ServiceBase
    {

        Helper helper = Helper.CreateInstance();
        public FileWatcherService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                helper.Logger.Log("File Watcher Service started...");
                EstablishFileWatcher();
            }
            catch (Exception ex)
            {
                helper.Logger.Log($"File Watcher Exception: {ex.ToString()}");
            }
        }

        private void EstablishFileWatcher()
        {
            string directoryPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"..\..\FileListner\");
            FileSystemWatcher watcher = new FileSystemWatcher(directoryPath);
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher.Filter = "*.txt";
            watcher.Changed += new FileSystemEventHandler(OnDirectoryChange);
            watcher.Created += new FileSystemEventHandler(OnDirectoryChange);
            watcher.EnableRaisingEvents = true;
        }

        private void OnDirectoryChange(object sender, FileSystemEventArgs e)
        {
            try
            {
                if (File.Exists(e.FullPath))
                {
                    helper.Logger.Log($"Start processing file: {e.FullPath}");
                    string messages;
                    const Int32 BufferSize = 128;
                    using (var fileStream = File.OpenRead(e.FullPath))
                    using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
                    {
                        VirtualXML virtualXML = new VirtualXML("Messages");
                        string line;
                        while ((line = streamReader.ReadLine()) != null)
                        {
                            string[] lineTokes = line.Split(',');
                            if (lineTokes.Count() == 2)
                            {
                                virtualXML.AddNode("Message", new Dictionary<string, string> { { "Data", lineTokes[0] }, { "IsActive", lineTokes[1] } });
                            }
                        }
                        messages = virtualXML.Get();
                    }
                    Dictionary<string, string> parameters = new Dictionary<string, string> { { "@Messages", messages } };
                    string conString = helper.GetAppKey("conStr");
                    ExecResult execResult = helper.DBConsumer.CallSQLDB(new DBExecParams() { ConString = conString, StoredProcedure = "spBulkEnqueueMessages", Parameters = parameters, ExecType = DBExecType.ExecuteNonQuery });
                    if (execResult.ErrorCode == 0)
                    {
                        helper.Logger.Log(execResult.ExecutionMessages);
                        Thread.Sleep(1000);
                        File.Delete(e.FullPath);
                        helper.Logger.Log($"End processing file, {execResult.AffectedRowsCount} messages insrted.");
                    }
                }
            }
            catch (Exception ex)
            {
                helper.Logger.Log($"Exception {ex.ToString()}");
            }
        }

        protected override void OnStop()
        {
            helper.Logger.Log("File Watcher Service stopped...");
        }
    }
}
