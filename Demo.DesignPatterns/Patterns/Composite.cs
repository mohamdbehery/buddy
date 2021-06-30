using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Patterns
{
    public class Composite
    {
        public void ConsumeThePattern()
        {
            ICProduct p1 = new CProduct("P1");
            ICProduct p2 = new CProduct("P2");
            ICProduct p3 = new CProduct("P3");
            ICProduct p4 = new CProduct("P4");
            ICProduct s1 = new CService("S1");
            ICProduct s2 = new CService("S2");

            ICProduct pp1 = new CProduct("PP1") { childs = { p1, p2, s1} };
            ICProduct pp2 = new CProduct("PP2") { childs = { p3, p4, s2 } };

            ICProduct ppp1 = new CProduct("PPP1") { childs = { pp1, pp2 } };

            ppp1.GetDetails();
        }
    }

    public interface ICProduct
    {
        void GetDetails();
    }

    public class CProduct : ICProduct
    {
        public List<ICProduct> childs; // this is the composite point
        public string Name { get; set; }

        public CProduct(string name)
        {
            this.Name = name;
            this.childs = new List<ICProduct>();
        }
        public void GetDetails()
        {
            Console.WriteLine($"Product: {this.Name}");
            foreach (var child in childs)
            {
                child.GetDetails();
            }
        }
    }

    public class CService : ICProduct
    {
        public string Name { get; set; }

        public CService(string name)
        {
            this.Name = name;
        }
        public void GetDetails()
        {
            Console.WriteLine($"Service: {this.Name}");
        }
    }
}
