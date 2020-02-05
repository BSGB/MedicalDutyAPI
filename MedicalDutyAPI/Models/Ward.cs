using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MedicalDutyAPI.Models
{
    [Table("wards")]
    public class Ward
    {
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("type")]
        public int Type { get; set; }

        [Column("hospital_id")]
        public int HospitalId { get; set; }
        
        [JsonIgnore]
        public Hospital Hospital { get; set; }

        public List<User> Users { get; set; }
    }
}
