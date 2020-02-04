using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
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

        public Hospital Hospital { get; set; }

        public List<User> Users { get; set; }
    }
}
