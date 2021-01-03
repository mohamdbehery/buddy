using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace App.Data.EFCore.ConceptualModels
{
    [Table("Infra.Service")]
    public class InfraService
    {
        [Key]
        public int Id { get; set; }
        public int ServiceClassID { get; set; }
        public int ModelClassID { get; set; }
        public string Code { get; set; }
        public string MethodName { get; set; }
        public int CachingTypeID { get; set; }
        public int AccessTypeID { get; set; }
        public string? Description { get; set; }
    }
}
