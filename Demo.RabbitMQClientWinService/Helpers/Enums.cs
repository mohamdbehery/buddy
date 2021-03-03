using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQClientWinService.Helpers
{
    public class Enums
    {
        public enum MQMessageState : int
        {
            Unknown = 0,
            SuccessfullyProcessed = 1,
            UnsuccessfulProcessing = 2,
            MessageRejected = 3
        }
    }
}
