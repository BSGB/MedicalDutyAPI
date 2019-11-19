using System;
using System.Collections.Generic;

namespace MedicalDutyAPI.Models
{
    public enum RoleEnum
    {
        Doctor,
        Headmaster,
        Admin
    }

    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public string Email { get; set; }
        public List<RoleEnum> Roles { get; set; }
        public long CreatedAt { get; set; }
    }
}
