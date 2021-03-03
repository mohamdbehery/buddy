using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buddy.Utilities
{
    public struct HelperEnums
    {
        public enum DBExecType
        {
            ExecuteNonQuery = 1,
            ExecuteScalar = 2,
            DataAdapter = 3
        }
        public enum ErrorCode: int
        {
            Zero = 0,
            Exception = 100
        }
    }
}
