using System;

namespace App.Data.LogicalModelsDTO
{
    public class AppUserModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string FamilyName { get; set; }
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
