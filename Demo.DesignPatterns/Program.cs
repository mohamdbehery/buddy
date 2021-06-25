using DesignPatterns.Patterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns
{
    class Program
    {        
        static void Main(string[] args)
        {

            #region Cascade Pattern CP
            // cascading brief code to not use object of class every time i call method inside it.
            //new CP().to("ffff").from("fgg").subject("ddd").message("rrr").send();
            #endregion

            #region Plugable Behavior Pattern PBP
            // instead of duplicate code in methods i can send a block of code as a method by using Func<>
            // instead of create a method for TotalOddVaues and Even vlues and all values i send condition method in a parameter
            //Console.WriteLine(new PBP().TotalSelectedValues(new int[] { 1, 2, 3, 4, 5 }, (value) => true)); // all values
            //Console.WriteLine("______________________");
            //Console.WriteLine(new PBP().TotalSelectedValues(new int[] { 1, 2, 3, 4, 5 }, (value) => value % 2 == 0)); // even values
            //Console.WriteLine("______________________");
            //Console.WriteLine(new PBP().TotalSelectedValues(new int[] { 1, 2, 3, 4, 5 }, (value) => value % 2 != 0)); // odd values
            //Console.ReadLine();
            #endregion

            #region Execute Around Method Pattern EAMP
            //EAMP_Resource.Use(
            //    (Resource) => {
            //        Resource.Op1();
            //        Resource.Op2();
            //    });
            //Console.WriteLine("out of block...");
            #endregion


            Console.ReadLine();
        }
    }
    public class PBP
    {
        public int TotalSelectedValues(int[] values, Func<int, bool> selector)
        {
            int total = 0;
            foreach (int value in values)
            {
                if (selector(value))
                    total += value;
            }
            return total;
        }
    }
    public class EAMP_Resource
    {
        // constructor
        // protected constructor >> cant create an object of it's class outside it
        protected EAMP_Resource()
        {
            Console.WriteLine("Creating...");
        }
        public void Op1()
        {
            Console.WriteLine("op1...");
        }
        public void Op2()
        {
            Console.WriteLine("op2...");
        }
        // destructor
        ~EAMP_Resource()
        {
            CleanUp();
        }
        public void CleanUp()
        {
            Console.WriteLine("Cleanup Expensive Resource.");
            Console.ReadLine();
        }        

        public static void Use(Action<EAMP_Resource> block)
        {
            EAMP_Resource res = new DesignPatterns.EAMP_Resource();
            try
            {
                block(res);
            }
            finally
            {


            }
        }
    }
}
