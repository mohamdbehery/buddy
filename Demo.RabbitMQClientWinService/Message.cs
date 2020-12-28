using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQClientWinService
{
    public class Message
    {
        public int MessageID { get; set; }
        public DateTime FetchDate { get; set; }
        public DateTime QueueDate { get; set; }
        public string MessageData { get; set; }
        public DateTime ExecuteDate { get; set; }
        public string MSBatchID { get; set; }
        public DateTime SuccessDate { get; set; }
        public DateTime FailureDate { get; set; }
        public string FailureMessage { get; set; }
        public bool IsActive { get; set; }
    }
}
