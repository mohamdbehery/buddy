using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Patterns
{
    public class FactoryMethod
    {
        public void ConsumeThePattern()
        {
            IFMCreator creator = new FMConcreteCreatorA();
            IFMProduct fMProduct = creator.FactoryMethod("A");
        }
    }

    public interface IFMCreator
    {
        IFMProduct FactoryMethod(string type);
    }
    public class FMConcreteCreatorA : IFMCreator
    {
        public IFMProduct FactoryMethod(string type)
        {
            switch (type)
            {
                case "A": return new FMProductA();
                case "B": return new FMProductB();
                default: throw new ArgumentException("Invalid type", "type");
            }
        }
    }

    public interface IFMProduct { }

    public class FMProductA : IFMProduct { }
    public class FMProductB : IFMProduct { }
}
