using Microsoft.EntityFrameworkCore;
using MedApp.Domain.Entities;
using MedApp.Domain.Enums;

namespace MedApp.Infrastructure.Persistence;

public class MedAppDbContext : DbContext
{
    public MedAppDbContext(DbContextOptions<MedAppDbContext> options) : base(options) { }

    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<Medicine> Medicines => Set<Medicine>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<PrescriptionDetail> PrescriptionDetails => Set<PrescriptionDetail>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // DateOnly conversions
        modelBuilder
            .Entity<Appointment>()
            .Property(p => p.AppointmentDate)
            .HasConversion(
                v => v.ToDateTime(TimeOnly.MinValue),
                v => DateOnly.FromDateTime(v));

        modelBuilder
            .Entity<PrescriptionDetail>()
            .Property(p => p.StartDate)
            .HasConversion(
                v => v.ToDateTime(TimeOnly.MinValue),
                v => DateOnly.FromDateTime(v));

        modelBuilder
            .Entity<PrescriptionDetail>()
            .Property(p => p.EndDate)
            .HasConversion(
                v => v.HasValue ? v.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null,
                v => v.HasValue ? DateOnly.FromDateTime(v.Value) : (DateOnly?)null);

        modelBuilder
            .Entity<Appointment>()
            .HasOne(a => a.Patient).WithMany().HasForeignKey(a => a.PatientId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder
            .Entity<Appointment>()
            .HasOne(a => a.Doctor).WithMany().HasForeignKey(a => a.DoctorId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Appointment>()
            .Property(a => a.VisitType)
            .HasConversion<int>();

        modelBuilder.Entity<PrescriptionDetail>()
            .HasOne(d => d.Medicine).WithMany().HasForeignKey(d => d.MedicineId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PrescriptionDetail>()
            .HasOne(d => d.Appointment).WithMany(a => a.Details).HasForeignKey(d => d.AppointmentId).OnDelete(DeleteBehavior.Cascade);

        // Seed master data
        modelBuilder.Entity<Patient>().HasData(
            new Patient { Id = 1, FullName = "John Doe", Email = "john@example.com", Phone = "111", IsActive = true },
            new Patient { Id = 2, FullName = "Jane Smith", Email = "jane@example.com", Phone = "222", IsActive = true }
        );
        modelBuilder.Entity<Doctor>().HasData(
            new Doctor { Id = 1, FullName = "Dr. Smith", Specialty = "General", IsActive = true },
            new Doctor { Id = 2, FullName = "Dr. Brown", Specialty = "Endocrinology", IsActive = true }
        );
        modelBuilder.Entity<Medicine>().HasData(
            new Medicine { Id = 1, Name = "Paracetamol", Unit = "500mg", IsActive = true },
            new Medicine { Id = 2, Name = "Metformin", Unit = "850mg", IsActive = true },
            new Medicine { Id = 3, Name = "Amoxicillin", Unit = "250mg", IsActive = true }
        );
    }
}
