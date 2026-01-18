using Microsoft.EntityFrameworkCore;

namespace HRMS.Models;

public class HRMSDbContext : DbContext
{
    public HRMSDbContext(DbContextOptions<HRMSDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<Department> Departments { get; set; }
    public DbSet<Position> Positions { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Document> Documents { get; set; }
    public DbSet<JobOffer> JobOffers { get; set; }
    public DbSet<Candidate> Candidates { get; set; }
    public DbSet<Interview> Interviews { get; set; }
    public DbSet<Salary> Salaries { get; set; }
    public DbSet<Bonus> Bonuses { get; set; }
    public DbSet<Benefit> Benefits { get; set; }
    public DbSet<EmployeeBenefit> EmployeeBenefits { get; set; }
    public DbSet<Equipment> Equipments { get; set; }
    public DbSet<EquipmentAssignment> EquipmentAssignments { get; set; }
    public DbSet<Promotion> Promotions { get; set; }
    public DbSet<User> Users { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Department
        modelBuilder.Entity<Department>()
            .HasOne(d => d.Manager)
            .WithMany()
            .HasForeignKey(d => d.ManagerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Department>()
            .HasMany(d => d.Employees)
            .WithOne(e => e.Department)
            .HasForeignKey(e => e.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Employee
        modelBuilder.Entity<Employee>()
            .Property(e => e.Matricule)
            .IsRequired()
            .HasMaxLength(20);

        modelBuilder.Entity<Employee>()
            .HasIndex(e => e.Matricule)
            .IsUnique();

        modelBuilder.Entity<Employee>()
            .HasOne(e => e.Position)
            .WithMany(p => p.Employees)
            .HasForeignKey(e => e.PositionId)
            .OnDelete(DeleteBehavior.Restrict);

        // Document
        modelBuilder.Entity<Document>()
            .HasOne(d => d.Employee)
            .WithMany(e => e.Documents)
            .HasForeignKey(d => d.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        // JobOffer
        modelBuilder.Entity<JobOffer>()
            .HasOne(j => j.Department)
            .WithMany(d => d.JobOffers)
            .HasForeignKey(j => j.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<JobOffer>()
            .HasOne(j => j.Position)
            .WithMany(p => p.JobOffers)
            .HasForeignKey(j => j.PositionId)
            .OnDelete(DeleteBehavior.Restrict);

        // Candidate / Interview
        modelBuilder.Entity<Candidate>()
            .HasOne(c => c.JobOffer)
            .WithMany(j => j.Candidates)
            .HasForeignKey(c => c.JobOfferId);

        modelBuilder.Entity<Interview>()
            .HasOne(i => i.Candidate)
            .WithMany(c => c.Interviews)
            .HasForeignKey(i => i.CandidateId);

        // Salary / Bonus
        modelBuilder.Entity<Salary>()
            .Property(s => s.BaseSalary)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Bonus>()
            .Property(b => b.Amount)
            .HasPrecision(18, 2);

        // Benefit
        modelBuilder.Entity<Benefit>()
            .Property(b => b.Value)
            .HasPrecision(18, 2);

        // Equipment
        modelBuilder.Entity<Equipment>()
            .Property(e => e.SerialNumber)
            .HasMaxLength(50);

        // Promotion
        modelBuilder.Entity<Promotion>()
            .HasOne(p => p.OldPosition)
            .WithMany()
            .HasForeignKey(p => p.OldPositionId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Promotion>()
            .HasOne(p => p.NewPosition)
            .WithMany()
            .HasForeignKey(p => p.NewPositionId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Promotion>()
            .Property(p => p.OldSalary)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Promotion>()
            .Property(p => p.NewSalary)
            .HasPrecision(18, 2);

        // User
        modelBuilder.Entity<User>()
            .HasOne(u => u.Employee)
            .WithOne(e => e.User)
            .HasForeignKey<User>(u => u.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .Property(u => u.Username)
            .IsRequired()
            .HasMaxLength(50);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();
    }
}