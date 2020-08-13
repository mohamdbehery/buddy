using System;
using System.Collections.Generic;
using System.Text;

namespace Buddy.Program
{
    public interface ITestInterface
    {
        string name { set; get; }

        int Sum(int x, int y);
        string SayHi();

    }
}
