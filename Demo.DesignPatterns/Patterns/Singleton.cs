using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Patterns
{
    public class Singleton
    {
        public void ConsumeThePattern()
        {
            var instance = TestLogger.GetInstance;
        }
    }

    public class TestLogger
    {
        private static TestLogger instance = null;
        private TestLogger() { }

        public static TestLogger GetInstance
        {
            get
            {
                return instance == null ? new TestLogger() : instance;
            }
        }
    }
}
