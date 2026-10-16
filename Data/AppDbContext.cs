using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Models;

namespace RaceDay.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Enrolment> Enrolments { get; set; }
    public DbSet<Result> Results { get; set; }
    public DbSet<Prize> Prizes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();

        modelBuilder.Entity<Result>()
            .HasOne(r => r.Enrolment)
            .WithOne(e => e.Result)
            .HasForeignKey<Result>(r => r.EnrolmentID)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Enrolment>()
            .HasOne(e => e.Event)
            .WithMany(ev => ev.Enrolments)
            .HasForeignKey(e => e.EventID)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Enrolment>()
            .HasOne(e => e.Participant)
            .WithMany(u => u.Enrolments)
            .HasForeignKey(e => e.ParticipantID)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Event>()
            .HasOne(e => e.Organiser)
            .WithMany(u => u.OrganisedEvents)
            .HasForeignKey(e => e.OrganiserID)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Prize>()
            .HasOne(p => p.Event)
            .WithMany(e => e.Prizes)
            .HasForeignKey(p => p.EventID)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Prize>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Prizes)
            .HasForeignKey(p => p.CategoryID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}