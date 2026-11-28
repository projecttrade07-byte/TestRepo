using EventApp.Models;
using Microsoft.EntityFrameworkCore;

namespace EventApp.Data;

public sealed class EventDbContext(DbContextOptions<EventDbContext> options) : DbContext(options)
{
    public DbSet<Event> Events => Set<Event>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Event>(entity =>
        {
            entity.Property(item => item.Title).HasMaxLength(120).IsRequired();
            entity.Property(item => item.Location).HasMaxLength(200).IsRequired();
            entity.Property(item => item.Description).HasMaxLength(1000);
            entity.Property(item => item.StartsAt).HasColumnType("datetime2");
            entity.Property(item => item.EndsAt).HasColumnType("datetime2");
        });
    }
}
