using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Buddy.API.Model
{
    public class ClassInfo
    {
        public Assembly AssemblyType { get; set; }
        public Type ClassType { get; set; }
        public object ClassInstance { get; set; }
    }
}
