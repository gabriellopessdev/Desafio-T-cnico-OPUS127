using Microsoft.EntityFrameworkCore;
using Opus127.Dengue.Api.Entities;

namespace Opus127.Dengue.Api.Data;

public sealed class DengueDbContext : DbContext
{
    public DengueDbContext(DbContextOptions<DengueDbContext> options) : base(options)
    {
    }

    public DbSet<DengueWeeklyAlert> DengueWeeklyAlerts => Set<DengueWeeklyAlert>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DengueWeeklyAlert>(entity =>
        {
            entity.ToTable("DengueWeeklyAlerts");
            entity.Property(e => e.EstimatedCases).HasPrecision(18, 2);
            entity.HasIndex(e => new { e.EpidemiologicalYear, e.EpidemiologicalWeek })
                .IsUnique();
        });
    }
}
