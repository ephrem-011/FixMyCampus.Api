using FixMyCampus.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FixMyCampus.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<Building> Buildings => Set<Building>();

    public DbSet<Ticket> Tickets => Set<Ticket>();

    public DbSet<TicketHistory> TicketHistories => Set<TicketHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(150);

            entity.HasIndex(x => x.Email)
                .IsUnique();

            entity.Property(x => x.Password)
                .IsRequired();

            entity.Property(x => x.Role)
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.Property(x => x.CreatedAt)
                .IsRequired();
        });

        modelBuilder.Entity<Building>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasIndex(x => x.Name)
                .IsUnique();
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Category)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.Room)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(1000);

            entity.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.Property(x => x.CreatedAt)
                .IsRequired();

            entity.Property(x => x.UpdatedAt)
                .IsRequired();

            entity.HasOne(x => x.Building)
                .WithMany(x => x.Tickets)
                .HasForeignKey(x => x.BuildingId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Reporter)
                .WithMany(x => x.ReportedTickets)
                .HasForeignKey(x => x.ReporterId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Technician)
                .WithMany(x => x.AssignedTickets)
                .HasForeignKey(x => x.TechnicianId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TicketHistory>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.FromStatus)
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.Property(x => x.ToStatus)
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.Property(x => x.ChangedAt)
                .IsRequired();

            entity.HasOne(x => x.Ticket)
                .WithMany(x => x.History)
                .HasForeignKey(x => x.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.ChangedBy)
                .WithMany(x => x.TicketHistories)
                .HasForeignKey(x => x.ChangedById)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}