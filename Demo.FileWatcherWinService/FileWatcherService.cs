using Buddy.Utilities;
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
using System.Threading.Tasks;

namespace FileWatcherWinService
{
    partial class FileWatcherService : ServiceBase
    {

        Helper helper = new Helper();
        public FileWatcherService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                helper.Log("File Watcher Service started...");
                EstablishFileWatcher();
            }
            catch (Exception ex)
            {
                helper.Log($"File Watcher Exception: {ex.ToString()}");
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
                    helper.Log($"Start processing file: {e.FullPath}");
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
                    int affectedRows = 0;
                    ReturnedData returnedData = helper.ExecuteSQLDB_SP(conString, "spBulkEnqueueMessages", parameters, out affectedRows);
                    if (returnedData.errorCode == 0)
                    {
                        File.Delete(e.FullPath);
                        helper.Log($"End processing file, {affectedRows} messages insrted.");
                    }
                    else
                    {
                        helper.Log($"Exception {returnedData.errorException}");
                    }
                }
            }
            catch (Exception ex)
            {
                helper.Log($"Exception {ex.ToString()}");
            }
        }

        protected override void OnStop()
        {
            helper.Log("File Watcher Service stopped...");
        }
    }
}
