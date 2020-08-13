using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Buddy.API.Model
{
    public class Response
    {
        public int Code { get; set; }
        public List<string> Messages { get; set; }
        public dynamic Results { get; set; }
    }
}
