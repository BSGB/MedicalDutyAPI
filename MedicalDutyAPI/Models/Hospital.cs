using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalDutyAPI.Models
{
    [Table("hospitals")]
    public class Hospital
    {
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("street")]
        public string Street { get; set; }

        [Column("zip")]
        public string Zip { get; set; }

        [Column("city")]
        public string City { get; set; }

        [Column("district")]
        public string District { get; set; }

        [Column("name")]
        public string Name { get; set; }

        public List<Ward> Wards { get; set; }
    }
}
