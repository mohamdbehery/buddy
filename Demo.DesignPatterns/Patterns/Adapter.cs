using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Patterns
{
    public class Adapter
    {
        public void ConsumeThePattern()
        {
            Adaptee adaptee = new Adaptee();
            IDataAdapter dataAdapter = new DataAdapter(adaptee);
            dataAdapter.PrintData();
        }
    }

    public interface IDataAdapter
    {
        void PrintData();
    }

    public class DataAdapter : IDataAdapter
    {
        readonly Adaptee _adaptee;
        public DataAdapter(Adaptee adaptee)
        {
            this._adaptee = adaptee;
        }
        public void PrintData()
        {
            _adaptee.PrintDifferentData();
        }
    }

    public class Adaptee
    {
        public void PrintDifferentData()
        {
            Console.WriteLine("Differnt Data");
        }
    }
}
