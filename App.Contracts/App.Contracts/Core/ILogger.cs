using System;
using System.Collections.Generic;
using System.Text;

namespace App.Contracts.Core
{
    public interface ILogger
    {
        string LogFilePath { get; }
        string LogsDirectory { get; set; }

        void Log(string logMessage);
    }
}
