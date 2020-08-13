using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Buddy.API.Model
{
    public class Request
    {
        public string ServiceCode { get; set; }
        public dynamic ServiceParam { get; set; }
    }
}
