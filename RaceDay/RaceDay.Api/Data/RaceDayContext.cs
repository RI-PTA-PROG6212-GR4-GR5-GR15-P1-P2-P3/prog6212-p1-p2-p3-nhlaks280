
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Models;

namespace RaceDay.Api.Data;

public class RaceDayContext : DbContext
{
    public RaceDayContext(DbContextOptions<RaceDayContext> options)
        : base(options)
    {
    }

    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<RaceEvent> Events => Set<RaceEvent>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<EventCategory> EventCategories => Set<EventCategory>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<Result> Results => Set<Result>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<AppUser>()
          .HasKey(u => u.UserID);
        modelBuilder.Entity<AppUser>().HasKey(u => u.UserID);
        modelBuilder.Entity<RaceEvent>().HasKey(e => e.EventID);
        modelBuilder.Entity<Category>().HasKey(c => c.CategoryID);
        modelBuilder.Entity<EventCategory>().HasKey(ec => ec.EventCategoryID);
        modelBuilder.Entity<Enrollment>().HasKey(e => e.EnrollmentID);
        modelBuilder.Entity<Result>().HasKey(r => r.ResultID);
        modelBuilder.Entity<RaceEvent>()
       .Property(e => e.DistanceKm)
       .HasPrecision(8, 2);

        modelBuilder.Entity<AppUser>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<AppUser>()
            .ToTable(t => t.HasCheckConstraint(
                "CK_Users_Role",
                "[Role] IN ('Organiser', 'Participant')"));

        modelBuilder.Entity<RaceEvent>()
            .HasOne(e => e.Organiser)
            .WithMany()
            .HasForeignKey(e => e.OrganiserID)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<EventCategory>()
            .HasIndex(ec => new { ec.EventID, ec.CategoryID })
            .IsUnique();

        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.Participant)
            .WithMany()
            .HasForeignKey(e => e.ParticipantID)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Enrollment>()
            .HasIndex(e => new { e.ParticipantID, e.EventCategoryID })
            .IsUnique();

        modelBuilder.Entity<Result>()
            .HasIndex(r => r.EnrollmentID)
            .IsUnique();

        modelBuilder.Entity<Result>()
            .Property(r => r.FinishTime)
            .HasColumnType("time");
    }
}
