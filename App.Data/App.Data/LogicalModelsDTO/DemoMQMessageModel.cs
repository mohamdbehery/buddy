using System;
using System.Collections.Generic;
using System.Text;

namespace App.Data.LogicalModelsDTO
{
    public class DemoMQMessageModel
    {
        public int Id { get; set; }
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
