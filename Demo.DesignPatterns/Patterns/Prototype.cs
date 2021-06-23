using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Patterns
{
    public class Prototype
    {
        public string Name { get; set; }
        public string PicturePath { get; set; }
        public int Count { get; set; }
        public string Factory { get; set; }
        public Prototype(string name)
        {
            this.Name = name;
        }

        public void ConsumeThePattern()
        {
            Prototype obj1 = new Prototype("");
            Prototype obj2 = new Prototype("");
            obj1.Name = "Old Value";
            obj2 = obj1; // cloning by ref, any changes will affect original object
            obj2.Name = "New Value";
            Console.WriteLine("With prototype " + obj1.Name); // will return "New Value"

            obj1 = new Prototype("");
            obj1.Name = "Old Value";
            obj2 = (Prototype)this.MemberwiseClone(); // cloning by value, any changes will NOT affect original object
            obj2.Name = "New Value";
            Console.WriteLine("With prototype " + obj1.Name); // will return "Old Value"
        }
    }
}
