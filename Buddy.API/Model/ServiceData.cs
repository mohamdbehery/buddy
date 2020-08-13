using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Buddy.API.Model
{
    public class ServiceMetaData
    {
        public string AssemblyClass { get; set; }
        public string ModelClass { get; set; }
        public string MethodName { get; set; }
        public int CashingType { get; set; }
        public int AccessType { get; set; }
    }
}
