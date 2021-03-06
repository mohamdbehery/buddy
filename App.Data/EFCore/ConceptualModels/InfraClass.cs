﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace App.Data.EFCore.ConceptualModels
{
    [Table("Infra.Class")]
    public class InfraClass
    {
        [Key]
        public int Id { get; set; }
        public int AssemblyID { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
    }
}
