using System;
using App.Data.EFCore;
using System.Linq;

namespace Buddy.Program
{
    class Program
    {
        static void Main(string[] args)
        {
            int i = 100;
            
            Console.WriteLine(i);

            ChangeValue(i);

            Console.WriteLine(i);

            Console.Read();
        }

        static void ChangeValue(int x)
        {
            x = 200;

            Console.WriteLine(x);
            return;
        }


    }
}
