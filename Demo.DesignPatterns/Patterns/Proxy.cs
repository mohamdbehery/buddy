using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Patterns
{
    public class Proxy
    {
        public void ConsumeThePattern()
        {
            var proxy = new OurProxy();
            proxy.Operation();
        }
    }

    public abstract class SubjectBase
    {
        public abstract void Operation();
    }

    public class RealSubject : SubjectBase
    {
        public override void Operation()
        {
            Console.WriteLine("RealSubject.Operation");
        }
    }

    public class OurProxy : SubjectBase
    {
        private RealSubject _realSubject;

        public override void Operation()
        {
            _realSubject = _realSubject == null ? new RealSubject() : _realSubject;
            _realSubject.Operation();
        }
    }
}
