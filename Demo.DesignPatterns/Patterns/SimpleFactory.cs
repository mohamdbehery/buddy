using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Patterns
{
    public class SimpleFactory
    {
        public void ConsumeThePattern()
        {
            ISFClass messageQueue = TestSimpleFactory.GetInstance("1");
        }
    }

    public static class TestSimpleFactory
    {
        public static ISFClass GetInstance(string type)
        {
            switch (type)
            {
                case "1": return new SFClass1();
                case "2": return new SFClass2();
                default: return null;
            }
        }
    }

    public interface ISFClass
    {

    }

    public class SFClass1 : ISFClass
    {

    }
    public class SFClass2 : ISFClass
    {

    }
}
