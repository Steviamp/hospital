using Microsoft.EntityFrameworkCore;
using PapageorgiouHospitalQueueSystem.Models;

namespace PapageorgiouHospitalQueueSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<DoctorsOffice> DoctorsOffices { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<PatientCall> PatientCalls { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patient>()
                .HasOne(p => p.CurrentDoctorsOffice)
                .WithMany()
                .HasForeignKey(p => p.CurrentDoctorsOfficeId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
