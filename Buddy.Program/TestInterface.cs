using System;
using System.Collections.Generic;
using System.Text;

namespace Buddy.Program
{
    public class TestInterface : ITestInterface
    {
        string _name = "";
        public string name
        {
            set { _name = value; }
            get { return _name; }
        }
        public TestInterface(string name)
        {
            this.name = name;
            SayHi();
        }

        public int Sum(int x, int y)
        {
            return x + y;
        }
        public string SayHi()
        {
            return "Hi " + name;
        }

        
    }
}
