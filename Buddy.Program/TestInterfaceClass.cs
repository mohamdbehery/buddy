using System;
using System.Collections.Generic;
using System.Text;

namespace Buddy.Program
{
    class TestInterfaceClass
    {
        TestInterface x = new TestInterface("Mohamed");
        public TestInterfaceClass()
        {
            x.SayHi();
        }
    }
}
