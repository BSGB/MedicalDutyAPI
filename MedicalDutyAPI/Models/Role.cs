using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MedicalDutyAPI.Models
{
    public enum RoleEnum
    {
        Administrator = 1,
        Headmaster = 2,
        Doctor = 3,
    }

    [Table("roles")]
    public class Role
    {
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("symbol")]
        public int Symbol { get; set; }

        [JsonIgnore]
        public List<UserRole> UserRoles { get; set; }
    }
}
