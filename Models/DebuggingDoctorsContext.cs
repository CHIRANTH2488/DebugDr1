using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Debugging_Doctors.Models;

public partial class DebuggingDoctorsContext : DbContext
{
    public DebuggingDoctorsContext()
    {
    }

    public DebuggingDoctorsContext(DbContextOptions<DebuggingDoctorsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<Doctor> Doctors { get; set; }

    public virtual DbSet<Patient> Patients { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:mycon");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.AppointmentId).HasName("PK__Appointm__8ECDFCA23EE75BFE");

            entity.Property(e => e.AppointmentId).HasColumnName("AppointmentID");
            entity.Property(e => e.AppointmentDate).HasColumnType("datetime");
            entity.Property(e => e.AppointmentStatus)
                .HasMaxLength(20)
                .HasColumnName("Appointment_Status");
            entity.Property(e => e.DoctorId).HasColumnName("DoctorID");
            entity.Property(e => e.InvoiceAmount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("Invoice_Amount");
            entity.Property(e => e.InvoiceStatus)
                .HasMaxLength(20)
                .HasColumnName("Invoice_Status");
            entity.Property(e => e.PatientId).HasColumnName("PatientID");

            entity.HasOne(d => d.Doctor).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Appointments_Doctors");

            entity.HasOne(d => d.Patient).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Appointments_Patients");
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.HasKey(e => e.DocId).HasName("PK__Doctors__3EF1888DA37925C5");

            entity.Property(e => e.DocId).HasColumnName("DocID");
            entity.Property(e => e.Availability).HasMaxLength(255);
            entity.Property(e => e.ContactNo).HasMaxLength(20);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Hpid)
                .HasMaxLength(14)
                .HasColumnName("HPID");
            entity.Property(e => e.Specialisation).HasMaxLength(100);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Doctors)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Doctors_Users");
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.PatientId).HasName("PK__Patients__970EC34615DF77BC");

            entity.Property(e => e.PatientId).HasColumnName("PatientID");
            entity.Property(e => e.AadhaarNo)
                .HasMaxLength(12)
                .HasColumnName("Aadhaar_no");
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.ContactNo).HasMaxLength(20);
            entity.Property(e => e.Dob).HasColumnName("DOB");
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Gender).HasMaxLength(20);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Patients)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Patients_Users");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC914B7926");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534CC37DC9A").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.PswdHash).HasMaxLength(255);
            entity.Property(e => e.Role).HasMaxLength(20);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
