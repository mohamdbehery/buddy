using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Buddy.Utilities.Enums.HelperEnums;

namespace Buddy.Utilities.Enums
{
    public static class Constants
    {
        public const ErrorCode DefaultErrorCode = ErrorCode.Zero;
        public const string DefaultLogExtention = ".txt";
        public const string LogFileNamePrefix = "Log";
        public const string UnknownProjectName = "UnknownSource";
        public const int LogFileMaxSizeInBytes = 2000000;
        public const string DefaultLogsDirectory = @"C:\Inetpub\BuddyLogger";
        public const string ADONETException = "ADO.NET Exception";
    }
}
