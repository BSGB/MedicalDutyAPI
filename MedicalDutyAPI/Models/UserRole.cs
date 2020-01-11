using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MedicalDutyAPI.Models
{
    [Table("users_to_roles")]
    public class UserRole
    {
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [JsonIgnore]
        public User User { get; set; }

        [Column("role_id")]
        public int RoleId { get; set; }

        public Role Role { get; set; }
    }
}
