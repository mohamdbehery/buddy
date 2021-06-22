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
            #region Abstract Factory Pattern AFP
            // Creates an instances of several families of classes
            //Console.WriteLine(AFP_Create("DesignPatterns.AFP_Book", new Dictionary<string, object>()
            //{
            //    { "Title", "Adventure" },
            //    { "Pages", 100}
            //}));
            //Console.WriteLine(AFP_Create("DesignPatterns.AFP_CD", new Dictionary<string, object>()
            //{
            //    { "Title", "Movie" },
            //    { "Volume", 2}
            //}));
            //Console.ReadLine();
            #endregion

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

            #region Builder Pattern
            //Product product = new Product("Shampoo");
            //product.setPicture("test").setCount(120).setFactory("Johnson");
            #endregion
        }

        public static object AFP_Create(string ClassName, Dictionary<string, object> values)
        {
            Type type = Type.GetType(ClassName);
            object Instance = Activator.CreateInstance(type);
            foreach (var entry in values)
            {
                type.GetProperty(entry.Key).SetValue(Instance, entry.Value);
            }
            return Instance;
        }
    }  
    
    public class Product
    {
        public string Name { get; set; }
        public string PicturePath { get; set; }
        public int Count { get; set; }
        public string Factory { get; set; }
        public Product(string name)
        {
            this.Name = name;
        }

        #region builder methods
        public Product setPicture(string picturePath)
        {
            this.PicturePath = picturePath;
            return this;
        }
        public Product setCount(int count)
        {
            this.Count = count;
            return this;
        }
        public Product setFactory(string factory)
        {
            this.Factory = factory;
            return this;
        }
        #endregion  
    }

    public class AFP_Book
    {
        public string Title { get; set; }
        public int Pages { get; set; }
        public override string ToString()
        {
            return string.Format("Book {0}, {1}", Title, Pages);
        }
    }
    public class AFP_CD
    {
        public string Title { get; set; }
        public int Volume { get; set; }
        public override string ToString()
        {
            return string.Format("CD {0}, {1}", Title, Volume);
        }
    }
    public class CP
    {
        public CP to(string toAdd)
        {
            return this;
        }
        public CP from(string fromAdd)
        {
            return this;
        }
        public CP subject(string sub)
        {
            return this;
        }
        public CP message(string mes)
        {
            return this;
        }
        public void send()
        {
            Console.WriteLine("message sent successfuly");
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
