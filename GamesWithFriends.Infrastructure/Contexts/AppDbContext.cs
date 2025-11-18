using GamesWithFriends.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace GamesWithFriends.Infrastructure.Contexts;

public sealed class AppDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Customer> Customers { get; set; } = null!;
    public DbSet<Notification> Notifications { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Entity>(options => { options.HasKey(entity => entity.Id); });

        modelBuilder.Entity<Customer>(options =>
        {
            options.Property(customer => customer.Username)
                .IsRequired()
                .HasMaxLength(32);

            options.HasIndex(customer => customer.Username)
                .IsUnique();

            options.Property(customer => customer.PasswordHash)
                .IsRequired();

            options.Property(customer => customer.Balance)
                .IsRequired()
                .HasDefaultValue(0);

            options.HasMany(customer => customer.Notifications)
                .WithOne(notification => notification.Receiver)
                .IsRequired();
        });

        modelBuilder.Entity<Notification>(options =>
        {
            options.HasOne(notification => notification.Receiver)
                .WithMany(customer => customer.Notifications);

            options.Property(notification => notification.Title)
                .IsRequired()
                .HasMaxLength(60);

            options.Property(notification => notification.Text)
                .IsRequired()
                .HasMaxLength(256);
        });
    }

    public override int SaveChanges()
    {
        UpdateTimestamps();

        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        UpdateTimestamps();

        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entities = ChangeTracker.Entries()
            .Where(x => x is
            {
                Entity: Entity,
                State: EntityState.Added
            });

        foreach (var entity in entities)
        {
            var now = DateTime.UtcNow;

            ((Entity)entity.Entity).CreatedAt = now;
        }
    }
}