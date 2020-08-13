using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace App.Data.EFCore.ConceptualModels
{
    public class AppUser
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string FamilyName { get; set; }
        public DateTime BirthDate { get; set; }
        public string eMailAddress { get; set; }
        public string LocationAddress { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public DateTime EntryDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
