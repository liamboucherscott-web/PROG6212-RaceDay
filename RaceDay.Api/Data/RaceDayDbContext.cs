using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Models;

namespace RaceDay.Api.Data;

public class RaceDayDbContext : DbContext
{
    public RaceDayDbContext(DbContextOptions<RaceDayDbContext> options) : base(options) { }

    public DbSet<Role> Roles { get; set; }
    public DbSet<EventType> EventTypes { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Enrolment> Enrolments { get; set; }
    public DbSet<Result> Results { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>().ToTable("Role");
        modelBuilder.Entity<EventType>().ToTable("EventType");
        modelBuilder.Entity<User>().ToTable("User");
        modelBuilder.Entity<Event>().ToTable("Event");
        modelBuilder.Entity<Category>().ToTable("Category");
        modelBuilder.Entity<Enrolment>().ToTable("Enrolment");
        modelBuilder.Entity<Result>().ToTable("Result");

        // Decimal precision
        modelBuilder.Entity<Event>().Property(e => e.Distance).HasPrecision(6, 2);
        modelBuilder.Entity<Category>().Property(c => c.EntryFee).HasPrecision(8, 2);

        modelBuilder.Entity<Enrolment>()
            .HasIndex(e => new { e.UserId, e.EventId, e.CategoryId })
            .IsUnique();

        modelBuilder.Entity<Enrolment>()
            .ToTable(t => t.HasCheckConstraint("CHK_Enrolment_Status", "[Status] IN ('Confirmed','Pending','Cancelled')"));

        modelBuilder.Entity<Result>()
            .HasOne(r => r.Enrolment)
            .WithOne(e => e.Result)
            .HasForeignKey<Result>(r => r.EnrolmentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Stop cascade delete to prevent multiple cascade paths
        modelBuilder.Entity<Enrolment>()
            .HasOne(e => e.User)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Enrolment>()
            .HasOne(e => e.Event)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Enrolment>()
            .HasOne(e => e.Category)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Role>().HasData(
            new Role { RoleId = 1, RoleName = "Organiser" },
            new Role { RoleId = 2, RoleName = "Participant" }
        );

        modelBuilder.Entity<EventType>().HasData(
            new EventType { EventTypeId = 1, TypeName = "Run" },
            new EventType { EventTypeId = 2, TypeName = "Walk" },
            new EventType { EventTypeId = 3, TypeName = "Cycle" }
        );
    }
}