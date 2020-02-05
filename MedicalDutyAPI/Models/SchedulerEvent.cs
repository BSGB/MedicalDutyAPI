using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MedicalDutyAPI.Models
{
    [Table("scheduler_events")]
    public class SchedulerEvent
    {
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("starts_at")]
        public DateTime StartsAt { get; set; }

        [Column("ends_at")]
        public DateTime EndsAt { get; set; }

        [Column("comment")]
        public string Comment { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [JsonIgnore]
        public User User { get; set; }
    }
}
