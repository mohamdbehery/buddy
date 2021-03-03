using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Buddy.Utilities.HelperEnums;

namespace Buddy.Utilities.Models
{

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
}
