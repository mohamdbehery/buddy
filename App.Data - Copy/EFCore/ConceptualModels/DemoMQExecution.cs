using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace App.Data.EFCore.ConceptualModels
{
    [Table("Demo.MQExecution")]
    public class DemoMQExecution
    {
        [Key]
        public int Id { get; set; }
        public int MessageID { get; set; }
        public string ExecutionResult { get; set; }
        public DateTime SuccessDate { get; set; }
        public string? IsActive { get; set; }
    }
}
