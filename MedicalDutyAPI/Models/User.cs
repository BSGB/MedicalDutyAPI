using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace MedicalDutyAPI.Models
{
    [Table("users")]
    public class User
    {
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("first_name")]
        public string FirstName { get; set; }

        [Column("last_name")]
        public string LastName { get; set; }

        [JsonIgnore]
        [Column("password")]
        public string Password { get; set; }

        [JsonIgnore]
        [Column("salt")]
        public string Salt { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        public List<UserRole> UserRoles { get; set; }

        public List<SchedulerEvent> SchedulerEvents { get; set; }
    }
}
