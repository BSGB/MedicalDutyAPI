using System;
using MedicalDutyAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MedicalDutyAPI
{
    public class DutyingContext : DbContext
    {
        public DbSet<User> Users {get; set;}
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<SchedulerEvent> SchedulerEvents { get; set; }
        public DbSet<Hospital> Hospitals { get; set; }
        public DbSet<Ward> Wards { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlServer(@"Server = localhost\SQLEXPRESS; Database=medical_duty;Trusted_Connection=True;");
    }
}
